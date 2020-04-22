using System;
using System.IO;
using System.Threading.Tasks;
using Lykke.Common;
using Microsoft.AspNetCore.Hosting;

namespace MAVN.Service.CustomerAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine($"{AppEnvironment.Name} version {AppEnvironment.Version}");
#if DEBUG
            Console.WriteLine("Is DEBUG");
#else
            Console.WriteLine("Is RELEASE");
#endif
            Console.WriteLine($"ENV_INFO: {AppEnvironment.EnvInfo}");

            try
            {
                var host = new WebHostBuilder()
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>()
                    .UseApplicationInsights()
                    .Build();

                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal error:");
                Console.WriteLine(ex);

                // Lets devops to see startup error in console between restarts in the Kubernetes
                var delay = TimeSpan.FromMinutes(1);

                Console.WriteLine();
                Console.WriteLine($"Process will be terminated in {delay}. Press any key to terminate immediately.");

                await Task.WhenAny(
                        Task.Delay(delay),
                        Task.Run(() =>
                        {
                            Console.ReadKey(true);
                        }));
            }

            Console.WriteLine("Terminated");
        }
    }
}
