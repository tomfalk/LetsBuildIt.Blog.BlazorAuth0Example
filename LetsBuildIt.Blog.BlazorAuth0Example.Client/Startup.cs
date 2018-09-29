using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using LetsBuildIt.Blog.BlazorAuth0Example.Client.Services;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace LetsBuildIt.Blog.BlazorAuth0Example.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<AppState>();
            services.AddSingleton<Auth0Service>();
            services.AddStorage();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
