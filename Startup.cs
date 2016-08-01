using System.IO;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using VidsNet.Classes;
using VidsNet.DataModels;
using VidsNet.Interfaces;
using VidsNet.Models;

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
            services.AddTransient<VideoScanner, VideoScanner>();
            services.AddTransient<SubtitleScanner, SubtitleScanner>();
            services.AddTransient<Scanner, Scanner>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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

            app.UseMvcWithDefaultRoute();
            
            /*app.Run(context =>
            {
                if(Setup.SettingsExist() && Setup.UsersExist()) {
                    return context.Response.WriteAsync("Everything is set up correctly!");
                }
                else {
                    return context.Response.WriteAsync("Your installation is not set up yet, or corrupted." +
                        "<a href=\"#\">Please click here to set it up.</a>");
                }
                
            });*/
        }
    }
}
