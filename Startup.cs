using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace VidsNet
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services){
            services.AddRouting();
            services.AddLogging();
            services.AddMvc();
            //services.AddEntityFrameworkSqlite();
            services.AddSession();
            services.AddDistributedMemoryCache();
            //TODO: configure
            services.AddAntiforgery();
            //TODO: configure
            services.AddAuthorization();
            //TODO: configure
            services.AddCors();
            //TODO: configure
            services.AddDataProtection();
            
            services.AddDbContext<DatabaseContext>();
            //services.AddTransient<VideoScanner, VideoScanner>();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {  
            app.UseSession();    
            app.UseStaticFiles(new StaticFileOptions() {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"public")),
                RequestPath = new PathString("/public")
            });     
            loggerFactory.AddConsole(LogLevel.Information);
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
