using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace VidsNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
                
            var configuration = builder.Build();

            var outPort = Int32.Parse(configuration.GetSection("Connectivity:OutPort").Value);
            var bindLocalhost = Convert.ToBoolean(configuration.GetSection("Connectivity:BindLocalhost").Value);

            var hostname = Dns.GetHostName();
            var ips = Dns.GetHostAddressesAsync(hostname).Result;
            
            var urls = new List<string>();
            foreach (var ip in ips)
            {
                var url = string.Format("http://{0}:{1}/", ip.ToString(), outPort);
                if(!urls.Contains(url)) {
                    urls.Add(url);
                }
            }

            if(bindLocalhost) {
                var localhost = string.Format("http://{0}:{1}/", "localhost", outPort);
                urls.Add(localhost);
                var loopback = string.Format("http://{0}:{1}/", "127.0.0.1", outPort);
                urls.Add(loopback);
            }

            var host = new WebHostBuilder().UseKestrel().UseUrls(urls.ToArray())
            .UseContentRoot(Directory.GetCurrentDirectory()).UseStartup<Startup>().Build();
            host.Run();
        }
    }
}
