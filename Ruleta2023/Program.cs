using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Ruleta2023
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error starting app");
                Console.WriteLine(e);
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())                
                .ConfigureServices(services => { services.AddAutofac(); })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}