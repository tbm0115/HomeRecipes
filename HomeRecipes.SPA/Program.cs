using HomeRecipes.SPA;
using HomeRecipes.SPA.Data;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    };

    client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
    client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
    client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("br"));
    return client;
});
builder.Services.AddScoped<LocalRecipeStore>();
builder.Services.AddSingleton<SyncState>();
builder.Services.AddBlazorBootstrap();
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Oidc", options.ProviderOptions);
    options.ProviderOptions.DefaultScopes.Clear();
    foreach (var scope in builder.Configuration.GetSection("Oidc:Scopes").Get<string[]>())
    {
        options.ProviderOptions.DefaultScopes.Add(scope);
    }
    options.ProviderOptions.ResponseType = "id_token token"; // Ensure this is set to "code" for PKCE
});

await builder.Build().RunAsync();
