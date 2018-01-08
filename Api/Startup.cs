using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add HTTP Cors support
            services.AddCors();

            // Add framework services.
            services.AddMvc();

            var appSettings = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettings);

            // Add Session handling.
            services.AddSession((SessionOptions options) =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(60);
                options.CookieHttpOnly = true;
                options.CookieSecure = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                options.CookieName = "session";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            var settings = new AppSettings();
            Configuration.GetSection("AppSettings").Bind(settings);

            app.UseCors(options => options.WithOrigins(settings.WebSiteUrl).AllowAnyHeader().AllowAnyMethod().AllowCredentials());

			app.UseSession();
            app.UseMvc();

        }
    }
}