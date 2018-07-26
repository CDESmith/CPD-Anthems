using System;
using Microsoft.Azure.WebJobs;

namespace Anthems_WebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    public class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run.
        // These must be set with these names in WebJobs:-
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            config.Queues.MaxPollingInterval = TimeSpan.FromSeconds(5);

            var host = new JobHost(config);

            host.RunAndBlock();
        }
    }
}
