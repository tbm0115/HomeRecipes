//using HomeRecipes.Shared;
//using Microsoft.JSInterop;
//using System.Security.Claims;
//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Drive.v3;
//using Google.Apis.Services;
//using System.Text;
//using System.IO;
//using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

//namespace HomeRecipes.SPA.Data
//{
//    public class LocalRecipeStore
//    {
//        private readonly HttpClient httpClient;
//        private readonly IJSRuntime js;
//        private readonly IAccessTokenProvider tokenProvider;
//        private DriveService driveService;
//        private Google.Apis.Drive.v3.Data.File recipeFile;

//        public LocalRecipeStore(HttpClient httpClient, IJSRuntime js, IAccessTokenProvider tokenProvider)
//        {
//            this.httpClient = httpClient;
//            this.js = js;
//            this.tokenProvider = tokenProvider;
//        }

//        public async Task InitializeDriveServiceAsync()
//        {
//            var result = await tokenProvider.RequestAccessToken();
//            if (result.TryGetToken(out var token))
//            {
//                var credential = GoogleCredential.FromAccessToken(token.Value);
//                driveService = new DriveService(new BaseClientService.Initializer()
//                {
//                    HttpClientInitializer = credential,
//                    ApplicationName = "HomeRecipes",
//                });
//                recipeFile = await GetOrCreateFileAsync("recipes.json");
//            }
//        }

//        private async Task<Google.Apis.Drive.v3.Data.File> GetOrCreateFileAsync(string fileName)
//        {
//            var request = driveService.Files.List();
//            request.Q = $"name='{fileName}'";
//            var result = await request.ExecuteAsync();
//            var file = result.Files.FirstOrDefault();
//            if (file == null)
//            {
//                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
//                {
//                    Name = fileName,
//                    MimeType = "application/json"
//                };
//                var createRequest = driveService.Files.Create(fileMetadata);
//                file = await createRequest.ExecuteAsync();
//            }
//            return file;
//        }

//        private async Task<string> ReadFileContentAsync()
//        {
//            var request = driveService.Files.Get(recipeFile.Id);
//            var stream = new MemoryStream();
//            await request.DownloadAsync(stream);
//            stream.Seek(0, SeekOrigin.Begin);
//            using var reader = new StreamReader(stream);
//            return await reader.ReadToEndAsync();
//        }

//        private async Task UpdateFileContentAsync(string content)
//        {
//            var fileMetadata = new Google.Apis.Drive.v3.Data.File();
//            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
//            var updateRequest = driveService.Files.Update(fileMetadata, recipeFile.Id, stream, "application/json");
//            await updateRequest.UploadAsync();
//        }

//        public ValueTask<Recipe[]> GetOutstandingLocalEditsAsync()
//        {
//            return js.InvokeAsync<Recipe[]>(
//                $"{IndexedDb.PREFIX}.getAll", IndexedDb.LOCAL_STORE);
//        }

//        public async Task SynchronizeAsync()
//        {
//            await InitializeDriveServiceAsync();
//            await FetchChangesAsync();
//        }

//        public ValueTask SaveUserAccountAsync(ClaimsPrincipal user)
//        {
//            return user != null
//                ? PutAsync(IndexedDb.META_STORE, "userAccount", user.Claims.Select(c => new ClaimData { Type = c.Type, Value = c.Value }))
//                : DeleteAsync(IndexedDb.META_STORE, "userAccount");
//        }

//        public async Task<ClaimsPrincipal> LoadUserAccountAsync()
//        {
//            var storedClaims = await GetAsync<ClaimData[]>(IndexedDb.META_STORE, "userAccount");
//            return storedClaims != null
//                ? new ClaimsPrincipal(new ClaimsIdentity(storedClaims.Select(c => new Claim(c.Type, c.Value)), "appAuth"))
//                : new ClaimsPrincipal(new ClaimsIdentity());
//        }

//        public ValueTask<string[]> Autocomplete(string prefix)
//            => js.InvokeAsync<string[]>($"{IndexedDb.PREFIX}.autocompleteKeys", IndexedDb.SERVER_STORE, prefix, 5);

//        public async Task<Recipe> GetRecipe(string recipeName)
//            => await GetAsync<Recipe>(IndexedDb.LOCAL_STORE, recipeName)
//            ?? await GetAsync<Recipe>(IndexedDb.SERVER_STORE, recipeName);

//        public async Task<IEnumerable<Recipe>> GetAllRecipes()
//        {
//            IEnumerable<Recipe> recipes = await GetAllAsync<Recipe>(IndexedDb.LOCAL_STORE);
//            if (recipes?.Any() == false)
//            {
//                return await GetAllAsync<Recipe>(IndexedDb.SERVER_STORE);
//            }
//            return recipes;
//        }

//        public async ValueTask<DateTime?> GetLastUpdateDateAsync()
//        {
//            var value = await GetAsync<string>(IndexedDb.META_STORE, "lastUpdateDate");
//            return value == null ? (DateTime?)null : DateTime.Parse(value);
//        }

//        public ValueTask SaveRecipeAsync(Recipe recipe)
//        {
//            return PutAsync(IndexedDb.LOCAL_STORE, null, recipe);
//        }

//        async Task FetchChangesAsync()
//        {
//            if (js is IJSInProcessRuntime)
//            {
//                var json = await ReadFileContentAsync();
//                await js.InvokeVoidAsync($"{IndexedDb.PREFIX}.putAllFromJson", IndexedDb.SERVER_STORE, json);
//                await PutAsync(IndexedDb.META_STORE, "lastUpdateDate", DateTime.Now.ToString("o"));
//            }
//        }

//        ValueTask<T> GetAsync<T>(string storeName, object key)
//            => js.InvokeAsync<T>($"{IndexedDb.PREFIX}.get", storeName, key);

//        ValueTask<IEnumerable<T>> GetAllAsync<T>(string storeName)
//            => js.InvokeAsync<IEnumerable<T>>($"{IndexedDb.PREFIX}.getAll", storeName);

//        ValueTask PutAsync<T>(string storeName, object key, T value)
//            => js.InvokeVoidAsync($"{IndexedDb.PREFIX}.put", storeName, key, value);

//        ValueTask DeleteAsync(string storeName, object key)
//            => js.InvokeVoidAsync($"{IndexedDb.PREFIX}.delete", storeName, key);

//        class ClaimData
//        {
//            public string Type { get; set; }
//            public string Value { get; set; }
//        }
//        public static class IndexedDb
//        {
//            public const string PREFIX = "localRecipeStore";
//            public const string LOCAL_STORE = "localedits";
//            public const string SERVER_STORE = "serverdata";
//            public const string META_STORE = "metadata";
//        }
//    }
//}
