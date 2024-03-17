using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace PFDB
{
    namespace Logging
    {
        public static class PythonLogger
        {

            public static void setup()
            {
                ConfigurationBuilder configuration = new ConfigurationBuilder();
                configuration.SetBasePath(Directory.GetCurrentDirectory()).AddEnvironmentVariables();
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration.Build())
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File("pfdblog", shared: true, buffered: true)
                    .CreateLogger();

                Log.Logger.Information("App start");

                var host = Host.CreateDefaultBuilder()
                    .ConfigureServices((context,services) =>
                    {

                    })
                    .UseSerilog()
                    .Build();

            }


        }
    }
}
