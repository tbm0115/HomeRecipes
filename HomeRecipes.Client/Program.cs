using System;
using System.Net.Http;
using System.Threading.Tasks;
using HomeRecipes.Client.Data;

namespace HomeRecipes.Client
{
    public class Program
    {
        public static async Task Main(string[] args) {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<LocalRecipeStore>();
            builder.Services.AddSingleton<SyncState>();

            await builder.Build().RunAsync();
        }
    }
}
