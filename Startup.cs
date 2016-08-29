using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using VidsNet.Scanners;
using VidsNet.DataModels;
using VidsNet.Enums;
using VidsNet.Classes;
using VidsNet.Filters;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.EntityFrameworkCore;

namespace VidsNet
{
    public class Startup
    {
        IConfigurationRoot Configuration;
        public Startup(IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("settings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
                
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services){
            services.AddRouting();
            services.AddLogging();
            services.AddOptions();

            var databaseType = Configuration.GetSection("Database:Type").Value;
            var connectionString = Configuration.GetSection("Database:ConnectionString").Value;
            var dbEnum = (Database)Enum.Parse(typeof(Database), databaseType);

            if(dbEnum == Database.Sqlite){
                services.AddEntityFrameworkSqlite();
                services.AddDbContext<DatabaseContext>(options => options.UseSqlite(connectionString));
            }
            else {              
                services.AddEntityFrameworkMySql();
                services.AddDbContext<DatabaseContext>(options => options.UseMySql(connectionString));
            }
            services.AddSingleton<DatabaseContext, DatabaseContext>();
            
            services.AddSession(options => {
                options.CookieName = ".VidsNet.Session";
            });
            services.AddDistributedMemoryCache();

            services.AddAuthorization(options => {
                options.AddPolicy("Default", policy => {
                    policy.AddAuthenticationSchemes("Automatic");
                    policy.RequireAuthenticatedUser();
                });
            });

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");          
            services.AddMvc( options => {
                options.Filters.Add(typeof(UserLoggedInFilter));
                options.Filters.Add(typeof(IsInstalledFilter));
            });

            services.AddScoped<BaseUserRepository, UserRepository>();
            services.AddSingleton<UserData, UserData>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddTransient<BaseScanner, BaseScanner>();
            services.AddTransient<Scanner, Scanner>();
            services.AddTransient<Video, Video>();
            services.AddTransient<Subtitle, Subtitle>();
            services.AddTransient<VideoViewer, VideoViewer>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {  
            app.UseSession();    
            app.UseStaticFiles(new StaticFileOptions() {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"public")),
                RequestPath = new PathString("/public")
            });     
            
            loggerFactory.AddConsole(LogLevel.Information);

            app.UseCookieAuthentication(new CookieAuthenticationOptions() {
                AuthenticationScheme = "Cookie",
                LoginPath = new PathString("/Account/Login/"),
                AccessDeniedPath = new PathString("/Account/Forbidden/"),
                CookieName = "VidsNETCookie",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseMvc(routes => {
                routes.MapRoute(name: "Default",
                template: "{controller}/{action}/{id?}",
                defaults: new { controller = "Home", action = "Index" });


                routes.MapRoute(name: "ItemView",
                template: "item/view/{session}/{id}/{name}");
                //template: "{controller}/{action}/{id}/{name}",
                //defaults: new { controller = "Item", action = "View" });

                routes.MapRoute(name: "Login",
                template: "Account/Login/");
                
            });

            app.UseExceptionHandler("/error");
        }
    }
}
