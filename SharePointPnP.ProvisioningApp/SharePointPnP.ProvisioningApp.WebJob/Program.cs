//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//
namespace SharePointPnP.ProvisioningApp.WebJob
{
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    class Program
    {
        public static async Task Main(string[] args)
        {
            /*
            var configuration = new JobHostConfiguration();
            // configuration.Queues.BatchSize = 1;
            configuration.Queues.MaxDequeueCount = 1;

            var options = new JobHostOptions();

            var host = new JobHost(options, null);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
            */

            // https://github.com/Azure/azure-webjobs-sdk/issues/1870#issuecomment-417888043

            var builder = new HostBuilder();

            builder.UseEnvironment("Development");

            builder.ConfigureWebJobs(b =>
            {
                b.UseHostId("ecad61-62cf-47f4-93b4-6efcded6");
                // b.AddWebJobsLogging(); // Enables WebJobs v1 classic logging 
                b.AddAzureStorageCoreServices();
                // b.AddAzureStorage();
                // b.AddAzureStorageBlobs(x =>
                // {
                //     // x.MaxDegreeOfParallelism = ...;
                // });
                b.AddAzureStorageQueues(x =>
                {
                    // x.
                });
                b.AddServiceBus();
                b.AddEventHubs();
            });

            builder.ConfigureAppConfiguration(b =>
            {
                // Adding command line as a configuration source
                b.AddCommandLine(args);
            });

            builder.ConfigureLogging((context, b) =>
            {
                b.SetMinimumLevel(LogLevel.Debug);
                b.AddConsole();

                // If this key exists in any config, use it to enable App Insights
                string appInsightsKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
                if (!string.IsNullOrEmpty(appInsightsKey))
                {
                    b.AddApplicationInsightsWebJobs(x =>
                    {
                        x.InstrumentationKey = appInsightsKey;
                    });
                }
            });

            builder.UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}
