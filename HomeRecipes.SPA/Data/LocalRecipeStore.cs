using HomeRecipes.Shared;
using Microsoft.JSInterop;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Google.Apis.Auth.OAuth2.Flows;
using System.Security.Claims;

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
            try
            {
                var result = await tokenProvider.RequestAccessToken(new AccessTokenRequestOptions { Scopes = new[] { "https://www.googleapis.com/auth/drive.appdata" } });
                if (result.TryGetToken(out var token))
                {
                    var applicationName = "LightWorks Home Recipes";
                    var tokenResponse = new TokenResponse
                    {
                        AccessToken = token.Value,
                        RefreshToken = token.RefreshToken // Use your refresh token here if needed
                    };

                    var credential = new UserCredential(new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = new ClientSecrets
                        {
                            // TODO: Add client info
                        },
                        Scopes = new[] { DriveService.Scope.DriveAppdata },
                        DataStore = new FileDataStore(applicationName)
                    }), "user", tokenResponse);

                    driveService = new DriveService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = applicationName,
                    });
                    recipeFile = await GetOrCreateFileAsync("recipes.json");
                    logger.LogInformation("Drive service initialized successfully.");
                }
                else
                {
                    logger.LogError("Failed to get access token. Error: {Error}", result.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error initializing Drive service.");
            }
        }

        private async Task<Google.Apis.Drive.v3.Data.File> GetOrCreateFileAsync(string fileName)
        {
            try
            {
                var request = driveService.Files.List();
                request.Q = $"name='{fileName}' and trashed=false";
                request.Fields = "files(id, name)";
                var result = await request.ExecuteAsync();
                var file = result.Files.FirstOrDefault();
                if (file == null)
                {
                    var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = fileName,
                        MimeType = "application/json"
                    };
                    var createRequest = driveService.Files.Create(fileMetadata);
                    file = await createRequest.ExecuteAsync();
                    logger.LogInformation("Created new file in Google Drive.");
                }
                else
                {
                    logger.LogInformation("File already exists in Google Drive.");
                }
                return file;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting or creating file in Google Drive.");
                return null;
            }
        }

        private async Task<string> ReadFileContentAsync()
        {
            if (!await EnsureDriveServiceInitializedAsync())
                throw new Exception("Drive service not initialized");

            try
            {
                var request = driveService.Files.Get(recipeFile.Id);
                var stream = new MemoryStream();
                await request.DownloadAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading file content from Google Drive.");
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
                logger.LogInformation("File content updated in Google Drive.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating file content in Google Drive.");
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

        public async ValueTask SaveRecipeAsync(Recipe recipe)
        {
            // Validate the recipe name
            if (string.IsNullOrEmpty(recipe.Name))
            {
                throw new InvalidOperationException("Recipe name is required and cannot be null or empty.");
            }

            await PutAsync(IndexedDb.LOCAL_STORE, null, recipe);

            var json = await ReadFileContentAsync();
            var recipes = JsonSerializer.Deserialize<List<Recipe>>(json) ?? new List<Recipe>();
            recipes.Add(recipe);

            var updatedJson = JsonSerializer.Serialize(recipes);
            await UpdateFileContentAsync(updatedJson);
        }

        public async Task FetchChangesAsync()
        {
            var json = await ReadFileContentAsync();
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

        class ClaimData
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }

        public static class IndexedDb
        {
            public const string PREFIX = "localRecipeStore";
            public const string LOCAL_STORE = "localedits";
            public const string SERVER_STORE = "serverdata";
            public const string META_STORE = "metadata";
        }
    }
}
