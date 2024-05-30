using HomeRecipes.Shared;
using Microsoft.JSInterop;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using System.Security.Claims;
using Google.Apis.Auth.OAuth2.Flows;

namespace HomeRecipes.SPA.Data
{
    public class LocalRecipeStore
    {
        private readonly HttpClient httpClient;
        private readonly IJSRuntime js;
        private readonly IAccessTokenProvider tokenProvider;
        private readonly ILogger<LocalRecipeStore> logger;
        private DriveService driveService;
        private Google.Apis.Drive.v3.Data.File recipeFile;

        public LocalRecipeStore(HttpClient httpClient, IJSRuntime js, IAccessTokenProvider tokenProvider, ILogger<LocalRecipeStore> logger)
        {
            this.httpClient = httpClient;
            this.js = js;
            this.tokenProvider = tokenProvider;
            this.logger = logger;
        }

        private async Task<bool> EnsureDriveServiceInitializedAsync()
        {
            if (driveService == null || recipeFile == null)
            {
                await InitializeDriveServiceAsync();
            }
            return driveService != null && recipeFile != null;
        }

        private async Task InitializeDriveServiceAsync()
        {
            var result = await tokenProvider.RequestAccessToken();
            if (result.TryGetToken(out var token))
            {
                var applicationName = typeof(App).Assembly.GetCustomAttribute<PackageTitleAttribute>()?.Title;
                if (string.IsNullOrEmpty(applicationName))
                {
                    throw new ArgumentException("Application name is not set. Ensure the PackageTitleAttribute is correctly applied to the assembly.");
                }

                var tokenResponse = new TokenResponse
                {
                    AccessToken = token.Value,
                    //RefreshToken = token.RefreshToken
                };

                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                    },
                    Scopes = new[] { "https://www.googleapis.com/auth/drive.appdata" },
                    DataStore = new FileDataStore("LightWorks", true)
                });

                var credential = new UserCredential(flow, Environment.UserName, tokenResponse);

                driveService = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName
                });

                recipeFile = await GetOrCreateFileAsync("recipes.json");
                logger.LogInformation("Drive service initialized successfully.");
            }
            else
            {
                logger.LogError("Failed to get access token. Error: {Error}", result.ToString());
                throw new InvalidOperationException("User is not authenticated.");
            }
        }

        private async Task<Google.Apis.Drive.v3.Data.File> GetOrCreateFileAsync(string fileName)
        {
            try
            {
                var request = driveService.Files.List();
                request.Spaces = "appDataFolder";
                request.Q = $"name='{fileName}'";
                request.Fields = "files(id, name)";
                var result = await request.ExecuteAsync();
                var file = result.Files.FirstOrDefault();
                if (file == null)
                {
                    var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = fileName,
                        Parents = new List<string> { "appDataFolder" }
                    };
                    var createRequest = driveService.Files.Create(fileMetadata);
                    file = await createRequest.ExecuteAsync();
                    logger.LogInformation("Created new file in Google Drive appDataFolder.");
                }
                else
                {
                    logger.LogInformation("File already exists in Google Drive appDataFolder.");
                }
                return file;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting or creating file in Google Drive appDataFolder.");
                throw;
            }
        }

        private async Task<string> ReadFileContentAsync()
        {
            if (!await EnsureDriveServiceInitializedAsync())
                throw new Exception("Drive service not initialized");

            try
            {
                var request = driveService.Files.Get(recipeFile.Id);
                using var stream = new MemoryStream();
                await request.DownloadAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading file content from Google Drive appDataFolder.");
                throw;
            }
        }

        private async Task UpdateFileContentAsync(string content)
        {
            if (!await EnsureDriveServiceInitializedAsync())
                throw new Exception("Drive service not initialized");

            try
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File();
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                var updateRequest = driveService.Files.Update(fileMetadata, recipeFile.Id, stream, "application/json");
                await updateRequest.UploadAsync();
                logger.LogInformation("File content updated in Google Drive appDataFolder.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating file content in Google Drive appDataFolder.");
                throw;
            }
        }

        public ValueTask<Recipe[]> GetOutstandingLocalEditsAsync()
        {
            return js.InvokeAsync<Recipe[]>(
                $"{IndexedDb.PREFIX}.getAll", IndexedDb.LOCAL_STORE);
        }

        public async Task SynchronizeAsync()
        {
            await EnsureDriveServiceInitializedAsync();
            await FetchChangesAsync();
        }

        public ValueTask SaveUserAccountAsync(ClaimsPrincipal user)
        {
            return user != null
                ? PutAsync(IndexedDb.META_STORE, "userAccount", user.Claims.Select(c => new ClaimData { Type = c.Type, Value = c.Value }))
                : DeleteAsync(IndexedDb.META_STORE, "userAccount");
        }

        public async Task<ClaimsPrincipal> LoadUserAccountAsync()
        {
            var storedClaims = await GetAsync<ClaimData[]>(IndexedDb.META_STORE, "userAccount");
            return storedClaims != null
                ? new ClaimsPrincipal(new ClaimsIdentity(storedClaims.Select(c => new Claim(c.Type, c.Value)), "appAuth"))
                : new ClaimsPrincipal(new ClaimsIdentity());
        }

        public ValueTask<string[]> Autocomplete(string prefix)
            => js.InvokeAsync<string[]>($"{IndexedDb.PREFIX}.autocompleteKeys", IndexedDb.SERVER_STORE, prefix, 5);

        public async Task<Recipe> GetRecipe(string recipeName)
            => await GetAsync<Recipe>(IndexedDb.LOCAL_STORE, recipeName)
            ?? await GetAsync<Recipe>(IndexedDb.SERVER_STORE, recipeName);

        public async Task<IEnumerable<Recipe>> GetAllRecipes()
        {
            IEnumerable<Recipe> recipes = await GetAllAsync<Recipe>(IndexedDb.LOCAL_STORE);
            if (recipes?.Any() == false)
            {
                return await GetAllAsync<Recipe>(IndexedDb.SERVER_STORE);
            }
            return recipes;
        }

        public async ValueTask<DateTime?> GetLastUpdateDateAsync()
        {
            var value = await GetAsync<string>(IndexedDb.META_STORE, "lastUpdateDate");
            return value == null ? (DateTime?)null : DateTime.Parse(value);
        }

        public ValueTask SaveRecipeAsync(Recipe recipe)
            => PutAsync(IndexedDb.LOCAL_STORE, null, recipe);

        async Task FetchChangesAsync()
        {
            var json = await httpClient.GetStringAsync($"recipes/recipes.json?d={DateTime.UtcNow}"); // The d parameter helps ensure that the JSON is ALWAYS read and bypasses cache
            await js.InvokeVoidAsync($"{IndexedDb.PREFIX}.putAllFromJson", IndexedDb.SERVER_STORE, json);
            await PutAsync(IndexedDb.META_STORE, "lastUpdateDate", DateTime.Now.ToString("o"));
        }

        ValueTask<T> GetAsync<T>(string storeName, object key)
            => js.InvokeAsync<T>($"{IndexedDb.PREFIX}.get", storeName, key);

        ValueTask<IEnumerable<T>> GetAllAsync<T>(string storeName)
            => js.InvokeAsync<IEnumerable<T>>($"{IndexedDb.PREFIX}.getAll", storeName);

        ValueTask PutAsync<T>(string storeName, object key, T value)
            => js.InvokeVoidAsync($"{IndexedDb.PREFIX}.put", storeName, key, value);

        ValueTask DeleteAsync(string storeName, object key)
            => js.InvokeVoidAsync($"{IndexedDb.PREFIX}.delete", storeName, key);
    }

    public static class IndexedDb
    {
        public const string PREFIX = "localRecipeStore";
        public const string LOCAL_STORE = "localedits";
        public const string SERVER_STORE = "serverdata";
        public const string META_STORE = "metadata";
    }

    public class ClaimData
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
