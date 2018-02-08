using System;
using System.Threading.Tasks;
using Grains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;

namespace SiloPlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            Console.WriteLine("Silo starting...");

            var config = ClusterConfiguration.LocalhostPrimarySilo();
            config.AddMemoryStorageProvider();

            var builder = new SiloHostBuilder()
                .UseConfiguration(config)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Corn).Assembly).WithReferences())
                /*.ConfigureLogging(logging => logging.AddConsole())*/
                ;

            var host = builder.Build();
            await host.StartAsync();

            Console.WriteLine("Silo started.");


            Console.WriteLine("Press q to quit:");
            while (Console.ReadKey(intercept: true).KeyChar != 'q') ;

            await host.StopAsync();
        }
    }
}
