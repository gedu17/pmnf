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
using VidsNet.Interfaces;
using VidsNet.Models;
using VidsNet.Classes;

namespace VidsNet
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services){
            services.AddRouting();
            services.AddLogging();
            

            services.AddEntityFrameworkSqlite().AddDbContext<DatabaseContext>();
            services.AddSession(options => {
                options.CookieName = ".VidsNet.Session";
            });
            services.AddDistributedMemoryCache();

            services.AddAuthorization(options => {
                options.AddPolicy("Default", policy => {
                    policy.AddAuthenticationSchemes("Automatic");
                    policy.RequireAuthenticatedUser();
                });
                options.AddPolicy("Administrator", policy =>  {
                    policy.AddAuthenticationSchemes("Automatic");
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Administrator");
                });
            });

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            
            //TODO: configure if needed
            //services.AddCors();
            //TODO: configure if needed
            //services.AddDataProtection();
            
            services.AddDbContext<DatabaseContext>();
            services.AddMvc();

            services.AddScoped<IUserRepository, UserRepository>();

            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<VideoScanner, VideoScanner>();
            services.AddTransient<SubtitleScanner, SubtitleScanner>();
            services.AddTransient<Scanner, Scanner>();
            services.AddTransient<VideoType, VideoType>();
            services.AddTransient<SubtitleType, SubtitleType>();
            services.AddTransient<UserData, UserData>();
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
                template: "{controller}/{action}/{id}/{name}",
                defaults: new { controller = "Item", action = "View" });
                
            });
        }
    }
}
