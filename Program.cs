using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace VidsNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string[] urls = {"http://194.135.88.108:8080/", "http://localhost:8080/"};
            var host = new WebHostBuilder().UseKestrel().UseUrls(urls)
            .UseContentRoot(Directory.GetCurrentDirectory()).UseStartup<Startup>().Build();
            host.Run();
        }
    }
}
