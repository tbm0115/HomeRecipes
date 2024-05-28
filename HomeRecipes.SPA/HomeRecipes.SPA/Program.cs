using HomeRecipes.SPA.Components;
using HomeRecipes.SPA.Data;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace HomeRecipes.SPA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register HttpClient
            builder.Services.AddHttpClient();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents();

            //// Add authorization services
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "218719763651-tmqjna7j1jkle2af5eejb4ma2d4t7pqm.apps.googleusercontent.com";
                    options.ClientSecret = "";
                });;
            //builder.Services.AddAuthenticationCore();

            //// Add authentication and authorization services
            builder.Services.AddOidcAuthentication(options =>
            {
                options.ProviderOptions.ClientId = "218719763651-tmqjna7j1jkle2af5eejb4ma2d4t7pqm.apps.googleusercontent.com";
                options.ProviderOptions.Authority = "https://accounts.google.com";
                options.ProviderOptions.ResponseType = "id_token";
                options.ProviderOptions.DefaultScopes.Add("openid");
                options.ProviderOptions.DefaultScopes.Add("profile");
                options.ProviderOptions.DefaultScopes.Add("email");
            });

            // Add other services
            //builder.Services.AddScoped<LocalRecipeStore>();
            //builder.Services.AddSingleton<SyncState>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode();

            app.Run();
        }
    }
}
