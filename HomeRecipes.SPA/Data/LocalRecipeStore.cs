using HomeRecipes.Shared;
using Microsoft.JSInterop;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Text.Json;
using System.Text;
using System.Security.Claims;
using Google.Apis.Drive.v3.Data;
using System.Text.Json.Serialization;

namespace HomeRecipes.SPA.Data
{
    public class GoogleAppDataStorage
    {
        private readonly IAccessTokenProvider tokenProvider;
        private readonly TokenStorage tokenStorage;
        private DriveService driveService;
        private readonly ILogger<GoogleAppDataStorage> logger;
        private readonly Dictionary<string, string> supportedMimeTypes = new Dictionary<string, string>()
        {
            { "json", "application/json" },
            { "png", "image/png" },
            { "jpg", "image/jpg" },
            { "jpeg", "image/jpeg" },
            { "webp", "image/webp" }
        };

        private Google.Apis.Drive.v3.Data.File ProfileConfigFile;
        public ProfileConfiguration ProfileConfiguration { get; set; }
        private GoogleCredential credential;

        /// <summary>
        /// Indicates whether or not the user profile has initialized the application.
        /// </summary>
        public bool IsInitialized => ProfileConfigFile != null;

        public GoogleAppDataStorage(IAccessTokenProvider tokenProvider, TokenStorage tokenStorage, ILogger<GoogleAppDataStorage> logger)
        {
            this.tokenProvider = tokenProvider;
            this.tokenStorage = tokenStorage;
            this.logger = logger;
        }

        /// <summary>
        /// Ensures the Google Drive API is connected and initialized with the default profile config file.
        /// </summary>
        /// <returns>Flag for whether or not the Google Drive appData folder has been initialized.</returns>
        public async Task<bool> EnsureDriveServiceInitializedAsync()
        {
            if (driveService == null || ProfileConfigFile == null)
            {
                await InitializeDriveServiceAsync();
            }
            return driveService != null && ProfileConfigFile != null;
        }

        /// <summary>
        /// Establishes a connection to the user's Google Drive appData folder and ensures the default profile config file is created.
        /// </summary>
        /// <returns>awaitable task</returns>
        private async Task InitializeDriveServiceAsync()
        {
            try
            {
                var result = await tokenProvider.RequestAccessToken(new AccessTokenRequestOptions { Scopes = new[] { "https://www.googleapis.com/auth/drive.appdata" } });

                if (result.TryGetToken(out var token))
                {
                    await tokenStorage.SetItemAsync("AccessToken", token.Value);

                    var applicationName = "LightWorks Home Recipes";
                    this.credential = GoogleCredential.FromAccessToken(token.Value)
                        .CreateScoped(DriveService.Scope.Drive);//.DriveAppdata);

                    driveService = new DriveService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = applicationName,
                        GZipEnabled = false,
                    });

                    // Establish connection with the Profile configuration file by utilizing the refresh method
                    await Refresh();
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

        private enum DriveSpacesEnum
        {
            appDataFolder,
            drive
        }

        /// <summary>
        /// Filename to get or create (including the extension).
        /// </summary>
        /// <param name="fileName">Filename, including the extension.</param>
        /// <returns>Google Drive file contained in the users appData folder.</returns>
        private async Task<Google.Apis.Drive.v3.Data.File> GetOrCreateFileAsync(string fileName)
        {
            string supportedMimeType;
            string extension = Path.GetExtension(fileName).Remove(0, 1);
            if (!supportedMimeTypes.TryGetValue(extension, out supportedMimeType) || string.IsNullOrEmpty(supportedMimeType))
            {
                throw new Exception("Mime type not supported");
            }

            try
            {
                var driveSpace = DriveSpacesEnum.drive.ToString();
                var request = driveService.Files.List();
                request.Spaces = driveSpace;
                request.Q = $"name='{fileName}' and trashed=false";
                request.Fields = "files(id, name)";
                var result = await request.ExecuteAsync();
                var file = result.Files.FirstOrDefault();
                if (file == null)
                {
                    var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = fileName,
                        Parents = new List<string>() { "Recipes" }
                    };
                    FilesResource.CreateMediaUpload uploadRequest;
                    using (var stream = new MemoryStream())
                    {
                        // Uploads a blank file
                        uploadRequest = driveService.Files.Create(
                            fileMetadata, stream, supportedMimeType);
                        uploadRequest.Fields = "id";
                        await uploadRequest.UploadAsync();
                    }

                    file = uploadRequest.ResponseBody;
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

        /// <summary>
        /// Reads the contents of the file wtih the given Google Drive File ID
        /// </summary>
        /// <param name="fileId">Google Drive API v3 File ID</param>
        /// <returns>String contents of the Google Drive File</returns>
        /// <exception cref="Exception"></exception>
        private async Task<string> ReadFileContentAsync(string fileId)
        {
            if (!await EnsureDriveServiceInitializedAsync())
                throw new Exception("Drive service not initialized");

            try
            {
                var request = driveService.Files.Get(fileId);
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

        /// <summary>
        /// Reads the given Google Drive File and deserializes the contents to the provided generic type.
        /// </summary>
        /// <typeparam name="T">Object type to deserialize the JSON into.</typeparam>
        /// <param name="fileId">Google Drive File ID</param>
        /// <returns>Deserialized object</returns>
        /// <exception cref="Exception"></exception>
        private async Task<T> ReadAsync<T>(string fileId)
        {
            var json = await ReadFileContentAsync(fileId);
            if (string.IsNullOrEmpty(json))
                return default(T);

            var result = JsonSerializer.Deserialize<T>(json);
            if (result == null)
                throw new Exception("Failed to deserialize JSON to generic type");
            return result;
        }

        /// <summary>
        /// Uploads the provided contents to the given Google Drive File
        /// </summary>
        /// <param name="fileId">Google Drive File ID</param>
        /// <param name="contents">Contents of the Google Drive File</param>
        /// <param name="mimeType">Mime type of the Google Drive File</param>
        /// <returns>awaitable task</returns>
        /// <exception cref="Exception"></exception>
        private async Task UpdateFileContentAsync(string fileId, byte[] contents, string mimeType)
        {
            if (!await EnsureDriveServiceInitializedAsync())
                throw new Exception("Drive service not initialized");

            if (!supportedMimeTypes.Values.Contains(mimeType))
                throw new Exception("Mime type not supported");

            try
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File();
                using var stream = new MemoryStream(contents);
                var updateRequest = driveService.Files.Update(fileMetadata, fileId, stream, mimeType);
                await updateRequest.UploadAsync();
                logger.LogInformation("File content updated in Google Drive.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating file content in Google Drive.");
                throw;
            }
        }

        /// <summary>
        /// Updates a JSON formatted Google Drive File
        /// </summary>
        /// <typeparam name="T">Object type to serialize</typeparam>
        /// <param name="fileId">Google Drive File ID</param>
        /// <param name="contents">Object to serialize</param>
        /// <returns>awaitable task</returns>
        private async Task UpdateAsync<T>(string fileId, T contents)
        {
            string json = JsonSerializer.Serialize(contents);
            await UpdateFileContentAsync(fileId, Encoding.UTF8.GetBytes(json), "application/json");
        }

        /// <summary>
        /// Adds the given email address as a collaborator to the given Google Drive File with the specified role.
        /// </summary>
        /// <param name="collectionName">Google Drive File ID</param>
        /// <param name="email">Email address of the user to give permission to.</param>
        /// <param name="role">Role the given user has on the Google Drive File.</param>
        /// <returns>awaitable task</returns>
        public async Task AddCollaboratorAsync(string collectionName, string email, string role)
        {
            var collectionConfig = await GetCollectionConfig(collectionName);
            if (collectionConfig == null)
                throw new Exception("Could not find recipe collection configuration");

            var permission = new Permission
            {
                Type = "user",
                Role = role,
                EmailAddress = email
            };

            var request = driveService.Permissions.Create(permission, collectionConfig.FileId);
            await request.ExecuteAsync();
        }

        public async Task Refresh()
        {
            ProfileConfigFile = await GetOrCreateFileAsync("profile.json");
            if (ProfileConfigFile != null)
            {
                this.ProfileConfiguration = await ReadAsync<ProfileConfiguration>(ProfileConfigFile.Id);
                if (this.ProfileConfiguration == null)
                {
                    // Initialize the default collection
                    var defaultCollection = await InitializeDefaultCollection();

                    this.ProfileConfiguration = new ProfileConfiguration()
                    {
                        User = "", // TODO: Get from authenticated user.
                        Collections = new List<CollectionConfig>()
                    {
                        new CollectionConfig()
                        {
                            Name = "Default",
                            FileId = defaultCollection.FileId
                        }
                    }
                    };
                    await UpdateProfile();
                }
                logger.LogInformation("Drive service initialized successfully.");
            }
        }

        private async Task<CollectionConfig> InitializeDefaultCollection()
        {
            RecipeCollection defaultCollection = null;
            var defaultCollectionFile = await GetOrCreateFileAsync("default.json");
            if (defaultCollectionFile != null)
            {
                defaultCollection = await ReadAsync<RecipeCollection>(defaultCollectionFile.Id);
                if (defaultCollection == null)
                {
                    defaultCollection = new RecipeCollection()
                    {
                        Name = "Default",
                        Description = "My personal, offline recipe book"
                    };
                    await UpdateAsync(defaultCollectionFile.Id, defaultCollection);
                }
                return new CollectionConfig
                {
                    Name = "Default",
                    FileId = defaultCollectionFile.Id,
                    DateModifiedUtc = defaultCollection.DateModifiedUtc
                };
            }
            else
            {
                throw new Exception("Failed to create default collection configuration on Google Drive");
            }
        }

        /// <summary>
        /// Updates the current Profile configuration on the Google Drive
        /// </summary>
        /// <returns></returns>
        public async Task UpdateProfile()
            => await UpdateAsync(this.ProfileConfigFile.Id, this.ProfileConfiguration);

        public async Task<CollectionConfig?> GetCollectionConfig(string collectionName)
        {
            if (!await EnsureDriveServiceInitializedAsync())
                throw new Exception("Drive service not initialized");
            return this.ProfileConfiguration
                .Collections
                .FirstOrDefault(o => o.Name.Equals(collectionName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the Recipe Collection by the given name.
        /// </summary>
        /// <param name="collectionName">Name of the Recipe Collection</param>
        /// <returns>Deserialized RecipeCollection</returns>
        /// <exception cref="Exception"></exception>
        public async Task<RecipeCollection?> GetCollection(string collectionName)
        {
            var collectionConfig = await GetCollectionConfig(collectionName);
            if (collectionConfig == null)
            {
                logger.LogWarning("No Recipe collection by the name {CollectionName}", collectionName);
                return null;
            }

            return await ReadAsync<RecipeCollection>(collectionConfig.FileId);
        }

        /// <summary>
        /// Adds a new file or updates the existing configuration file for a specific Recipe Collection by name up to the Google Drive.
        /// </summary>
        /// <param name="collection">Object to serialize into the configuration file.</param>
        /// <returns>awaitable task</returns>
        /// <exception cref="Exception"></exception>
        public async Task AddOrUpdateCollection(RecipeCollection collection)
        {
            // Update collection meta
            collection.DateModifiedUtc = DateTime.UtcNow;

            var collectionConfig = await GetCollectionConfig(collection.Name);

            // Now it's time to update Google Drive, get the Collection config Google Drive File ID from the profile configuration
            string googleDriveFileId = collectionConfig
                ?.FileId
                ?? string.Empty;
            if (string.IsNullOrEmpty(googleDriveFileId))
            {
                // Collection file doesn't exist, so create one
                var file = await GetOrCreateFileAsync($"{collection.Name}.json");
                if (file != null)
                {
                    googleDriveFileId = file.Id;

                    // Update Profile config
                    ProfileConfiguration!.Collections.Add(new CollectionConfig()
                    {
                        Name = collection.Name,
                        FileId = file.Id,
                        DateModifiedUtc = collection.DateModifiedUtc
                    });
                    // Push updates to Google Drive
                    await UpdateProfile();
                }
                else
                {
                    throw new Exception("Couldn't create new Recipe collection file");
                }
            }
            else
            {
                // Update profile
                collectionConfig!.DateModifiedUtc = collection.DateModifiedUtc;
                await UpdateProfile();
            }
            // Push updates to Google Drive
            await UpdateAsync(googleDriveFileId!, collection);
        }

        public async Task DeleteCollection(RecipeCollection collection)
        {
            var collectionConfig = await GetCollectionConfig(collection.Name);

            if (collectionConfig != null)
            {
                string googleDriveFileId = collectionConfig.FileId;

                if (!string.IsNullOrEmpty(googleDriveFileId))
                {
                    try
                    {
                        // Delete the file from Google Drive
                        var deleteRequest = driveService.Files.Delete(googleDriveFileId);
                        await deleteRequest.ExecuteAsync();
                        logger.LogInformation($"Deleted Google Drive file with ID: {googleDriveFileId}");

                        // Remove the collection from the profile configuration
                        ProfileConfiguration.Collections.Remove(collectionConfig);

                        // Update the profile on Google Drive
                        await UpdateProfile();
                        logger.LogInformation($"Deleted collection '{collection.Name}' from profile configuration");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Error deleting collection '{collection.Name}' from Google Drive and profile configuration");
                    }
                }
                else
                {
                    logger.LogWarning($"Collection '{collection.Name}' does not have a valid Google Drive file ID");
                }
            }
            else
            {
                logger.LogWarning($"Collection '{collection.Name}' not found in profile configuration");
            }
        }
    }

    public class LocalRecipeStore
    {
        private readonly IJSRuntime js;
        private readonly ILogger<LocalRecipeStore> logger;

        private GoogleAppDataStorage googleService;

        public LocalRecipeStore(IJSRuntime js, GoogleAppDataStorage googleService, ILogger<LocalRecipeStore> logger)
        {
            this.js = js;
            this.googleService = googleService;
            this.logger = logger;
        }

        public async Task SaveCollectionAsync(RecipeCollection collection)
            => await googleService.AddOrUpdateCollection(collection);

        public async Task AddCollaboratorToCollectionAsync(string collectionName, string email, string role)
            => await googleService.AddCollaboratorAsync(collectionName, email, role);

        public ValueTask<RecipeCollection> GetOutstandingLocalEditsAsync()
        {
            return js.InvokeAsync<RecipeCollection>(
                $"{IndexedDb.PREFIX}.getAll", IndexedDb.COLLECTION_STORE);
        }

        public async Task SynchronizeAsync()
        {
            if (googleService.IsInitialized)
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
            => js.InvokeAsync<string[]>($"{IndexedDb.PREFIX}.autocompleteKeys", IndexedDb.COLLECTION_STORE, prefix, 5);

        public async Task<IEnumerable<RecipeCollection>> GetCollections()
            => await GetAllAsync<RecipeCollection>(IndexedDb.COLLECTION_STORE);

        public async Task<RecipeCollection?> GetCollection(string collectionName)
        {
            var collection = await GetAsync<RecipeCollection>(IndexedDb.COLLECTION_STORE, collectionName);
            if (collection == null)
            {
                var cloudCollection = await googleService.GetCollection(collectionName);
                if (cloudCollection != null)
                {
                    await PutAsync(IndexedDb.COLLECTION_STORE, null, cloudCollection);
                }
                else
                {
                    var ex = new Exception("Could not find local collection by name '" + collectionName + "'");
                    logger.LogError(ex, ex.Message);
                    return null;
                }
            }

            return collection;
        }

        public async Task<bool> HasRecipe(string collectionName, string recipeName)
        {
            return (await GetRecipe(collectionName, recipeName)) != null;
        }

        public async Task<Recipe?> GetRecipe(string collectionName, string recipeName)
        {
            var collection = await GetCollection(collectionName);
            return collection?.Recipes.FirstOrDefault(o => o.Name == recipeName);
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipes()
        {
            var collections = await GetCollections();
            return collections.SelectMany(o => o.Recipes);
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipes(string collectionName)
        {
            var collection = await GetCollection(collectionName);
            return collection?.Recipes ?? new List<Recipe>();
        }

        public async ValueTask<DateTime?> GetLastUpdateDateAsync()
        {
            var value = await GetAsync<string>(IndexedDb.META_STORE, "lastUpdateDate");
            return value == null ? (DateTime?)null : DateTime.Parse(value);
        }

        public async ValueTask SaveRecipeAsync(string collectionName, Recipe recipe)
        {
            // Get the Recipe Collection from the IndexedDB by name
            var collection = await GetCollection(collectionName);
            if (collection == null)
            {
                collection = new RecipeCollection() { Name = collectionName };
                await PutAsync(IndexedDb.COLLECTION_STORE, null, collection);
            }

            // Validate the recipe name
            if (string.IsNullOrEmpty(recipe.Name))
            {
                throw new InvalidOperationException("Recipe name is required and cannot be null or empty.");
            }

            // Lookup existing recipe or assign new recipe
            var record = collection
                .Recipes
                .FirstOrDefault(o => o.Name.Equals(recipe.Name, StringComparison.OrdinalIgnoreCase))
                ?? recipe;

            // TODO: Use AutoMapper or something
            record.Name = recipe.Name;
            record.Description = recipe.Description;
            record.PrepTime = recipe.PrepTime;
            record.CookTime = recipe.CookTime;
            record.RecipeYield = recipe.RecipeYield;
            record.ThumbnailUrl = recipe.ThumbnailUrl;
            record.Ingredients = recipe.Ingredients;
            record.Instructions = recipe.Instructions;

            recipe.DateModified = DateTime.UtcNow;
            recipe.DatePublished = DateTime.UtcNow;

            // Add Recipe if it doesn't exist
            if (!collection.Recipes.Any(o => o.Name.Equals(record.Name, StringComparison.OrdinalIgnoreCase)))
            {
                collection.Recipes.Add(record);
            }

            // Update IndexedDB with collection
            await PutAsync(IndexedDb.COLLECTION_STORE, null, collection);

            // Update Collection file on Google Drive
            await googleService.AddOrUpdateCollection(collection);
        }

        public async ValueTask DeleteCollectionAsync(string collectionName)
        {
            var collection = await GetCollection(collectionName);
            if (collection == null)
                throw new Exception("Could not find collection by name '" + collectionName + "'");
            await DeleteAsync(IndexedDb.COLLECTION_STORE, collectionName);
            // Delete Collection file on Google Drive
            await googleService.DeleteCollection(collection);
        }

        public async ValueTask DeleteRecipeAsync(string collectionName, Recipe recipe)
        {
            // Get the Recipe Collection from the IndexedDB by name
            var collection = await GetCollection(collectionName);
            if (collection == null)
            {
                collection = new RecipeCollection() { Name = collectionName };
                await PutAsync(IndexedDb.COLLECTION_STORE, null, collection);
            }

            // Validate the recipe name
            if (string.IsNullOrEmpty(recipe.Name))
            {
                throw new InvalidOperationException("Recipe name is required and cannot be null or empty.");
            }

            // Lookup existing recipe or assign new recipe
            var record = collection
                .Recipes
                .FirstOrDefault(o => o.Name.Equals(recipe.Name, StringComparison.OrdinalIgnoreCase))
                ?? recipe;

            // Add Recipe if it doesn't exist
            if (collection.Recipes.Any(o => o.Name.Equals(record.Name, StringComparison.OrdinalIgnoreCase)))
            {
                collection.Recipes.Remove(record);
            }

            // Update IndexedDB with collection
            await PutAsync(IndexedDb.COLLECTION_STORE, null, collection);

            // Update Collection file on Google Drive
            await googleService.AddOrUpdateCollection(collection);
        }

        public async Task FetchChangesAsync()
        {
            await googleService.Refresh();

            foreach (var profileCollection in googleService.ProfileConfiguration.Collections)
            {
                var localCollection = await GetCollection(profileCollection.Name);
                if (localCollection == null)
                {
                    if (profileCollection.Name.Equals("Default"))
                    {
                        localCollection = new RecipeCollection()
                        {
                            Name = "Default",
                            DateCreatedUtc = DateTime.UtcNow,
                            DateModifiedUtc = DateTime.UtcNow,
                        };
                        await PutAsync(IndexedDb.COLLECTION_STORE, null, localCollection);
                    }
                    else
                    {
                        continue;
                    }
                }

                if (profileCollection.DateModifiedUtc > localCollection.DateModifiedUtc)
                {
                    var driveCollection = await googleService.GetCollection(profileCollection.Name);
                    if (driveCollection == null)
                        throw new Exception("Failed to get Recipe Collection file from Google Drive");

                    await PutAsync(IndexedDb.COLLECTION_STORE, null, driveCollection);
                }
                else if (profileCollection.DateModifiedUtc < localCollection.DateModifiedUtc)
                {
                    await googleService.AddOrUpdateCollection(localCollection);
                }
            }

            var localCollections = await GetCollections();
            foreach (var localCollection in localCollections)
            {
                bool existsInDrive = googleService.ProfileConfiguration.Collections.Any(o => o.Name.Equals(localCollection.Name, StringComparison.OrdinalIgnoreCase));
                if (!existsInDrive)
                    await googleService.AddOrUpdateCollection(localCollection);
            }
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
            public const string META_STORE = "metadata";
            public const string COLLECTION_STORE = "collections";
        }
    }

}
