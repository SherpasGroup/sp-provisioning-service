//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Logging;
using System.Configuration;

namespace SharePointPnP.ProvisioningApp.WebJobServiceBus
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder();

            builder.UseEnvironment(ConfigurationManager.AppSettings["SPPA:ProvisioningEnvironment"]);

            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddServiceBus(sbOptions =>
                {
                    // ServiceBusOptions ConnectionString
                    // sbOptions.ConnectionString = ConfigurationManager.ConnectionStrings["AzureWebJobsServiceBus"].ConnectionString;

                    // sbOptions.MessageHandlerOptions.AutoComplete = true;
                    sbOptions.AutoCompleteMessages = true;

                    int maxConcurrentCalls;
                    if (!int.TryParse(ConfigurationManager.AppSettings["SB:MaxConcurrentCalls"], out maxConcurrentCalls))
                    {
                        maxConcurrentCalls = 32;
                    }
                    // sbOptions.MessageHandlerOptions.MaxConcurrentCalls = maxConcurrentCalls;
                    sbOptions.MaxConcurrentCalls = maxConcurrentCalls;

                    int maxAutoRenewDuration;
                    if (!int.TryParse(ConfigurationManager.AppSettings["SB:MaxAutoRenewDuration"], out maxAutoRenewDuration))
                    {
                        maxAutoRenewDuration = 45;
                    }
                    // sbOptions.MessageHandlerOptions.MaxAutoRenewDuration = TimeSpan.FromMinutes(maxAutoRenewDuration);
                    sbOptions.MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(maxAutoRenewDuration);
                });

                // b.AddServiceBusClient<IAzureClientFactoryBuilder>("connection string");
            });

            builder.ConfigureLogging((context, b) =>
            {
                b.AddConsole();

                // If the key exists in settings, use it to enable Application Insights.
                string instrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"] ?? context.Configuration["InstrumentationKey"];
                if (!string.IsNullOrEmpty(instrumentationKey))
                {
                    b.AddApplicationInsightsWebJobs(o =>
                    {
                        o.InstrumentationKey = instrumentationKey;
                    });
                }
            });

            var host = builder.Build();
            using (host)
            {
                host.Run();
            }
        }
    }
}
