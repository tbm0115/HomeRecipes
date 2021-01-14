using HomeRecipes.Shared;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HomeRecipes.Client.Data
{

    public class LocalRecipeStore
    {
        private readonly HttpClient httpClient;
        private readonly IJSRuntime js;

        public LocalRecipeStore(HttpClient httpClient, IJSRuntime js) {
            this.httpClient = httpClient;
            this.js = js;
        }

        public ValueTask<Recipe[]> GetOutstandingLocalEditsAsync() {
            return js.InvokeAsync<Recipe[]>(
                $"{IndexedDb.PREFIX}.getAll", IndexedDb.LOCAL_STORE);
        }

        public async Task SynchronizeAsync() {
            //// If there are local edits, always send them first
            //foreach (var editedRecipe in await GetOutstandingLocalEditsAsync()) {
            //    (await httpClient.PutAsJsonAsync("api/recipe/details", editedRecipe)).EnsureSuccessStatusCode();
            //    await DeleteAsync(IndexedDb.LOCAL_STORE, editedRecipe.Name);
            //}

            await FetchChangesAsync();
        }

        public ValueTask SaveUserAccountAsync(ClaimsPrincipal user) {
            return user != null
                ? PutAsync(IndexedDb.META_STORE, "userAccount", user.Claims.Select(c => new ClaimData { Type = c.Type, Value = c.Value }))
                : DeleteAsync(IndexedDb.META_STORE, "userAccount");
        }

        public async Task<ClaimsPrincipal> LoadUserAccountAsync() {
            var storedClaims = await GetAsync<ClaimData[]>(IndexedDb.META_STORE, "userAccount");
            return storedClaims != null
                ? new ClaimsPrincipal(new ClaimsIdentity(storedClaims.Select(c => new Claim(c.Type, c.Value)), "appAuth"))
                : new ClaimsPrincipal(new ClaimsIdentity());
        }

        public ValueTask<string[]> Autocomplete(string prefix)
            => js.InvokeAsync<string[]>($"{IndexedDb.PREFIX}.autocompleteKeys", IndexedDb.SERVER_STORE, prefix, 5);

        // If there's an outstanding local edit, use that. If not, use the server data.
        public async Task<Recipe> GetRecipe(string recipeName)
            => await GetAsync<Recipe>(IndexedDb.LOCAL_STORE, recipeName)
            ?? await GetAsync<Recipe>(IndexedDb.SERVER_STORE, recipeName);

        public async Task<IEnumerable<Recipe>> GetAllRecipes() {
            IEnumerable<Recipe> recipes = await GetAllAsync<Recipe>(IndexedDb.LOCAL_STORE);
            if (recipes?.Any() == false){
                return await GetAllAsync<Recipe>(IndexedDb.SERVER_STORE);
            }
            return recipes;
        }

        public async ValueTask<DateTime?> GetLastUpdateDateAsync() {
            var value = await GetAsync<string>(IndexedDb.META_STORE, "lastUpdateDate");
            return value == null ? (DateTime?)null : DateTime.Parse(value);
        }

        public ValueTask SaveRecipeAsync(Recipe recipe)
            => PutAsync(IndexedDb.LOCAL_STORE, null, recipe);

        async Task FetchChangesAsync() {
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
