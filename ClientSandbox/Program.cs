using System;
using System.Linq;
using System.Threading.Tasks;
using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;

namespace ClientSandbox
{
    class Program
    {
        static IClusterClient ClusterClient;

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            Console.WriteLine("Client Start.");

            await ConfigAndConnectToSilo();

            Console.WriteLine("Microwaving a bag...");

            for (var batchNumber = 0; batchNumber < 10000; batchNumber++)
            {
                // I'm doing this in batches to try to get them all created in under 2 minutes
                const int batchSize = 100;
                await Task.WhenAll(
                    Enumerable.Range(batchSize * batchNumber, batchSize)
                    .Select(n => ClusterClient.GetGrain<ICorn>(n))
                    .Select(c => c.BeginPop())
                );
            }

            Console.WriteLine("Microwaving started.");

            Console.WriteLine("Press q to quit:");
            while (Console.ReadKey(intercept: true).KeyChar != 'q') ;
        }

        static async Task ConfigAndConnectToSilo()
        {
            var config = ClientConfiguration.LocalhostSilo();

            do
            {
                try
                {
                    var builder = new ClientBuilder()
                        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ICorn).Assembly).WithReferences())
                        .ConfigureLogging(logging => logging.AddConsole())
                        .UseConfiguration(config);
                    ClusterClient = builder.Build();
                    Console.WriteLine("Connecting...");
                    await ClusterClient.Connect();
                    break;
                }
                catch (SiloUnavailableException)
                {
                    Console.WriteLine("Try connecting again I guess... =/ Do I really need to write this code?");
                    continue;
                }
            }
            while (true);

            Console.WriteLine(@"Connect completed \o/");
        }

    }
}
