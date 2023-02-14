﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution;
using SFA.DAS.EmployerAccounts.MessageHandlers.Extensions;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.MessageHandlers;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var builder = CreateHostBuilder(args);

            using var host = builder.Build();
            await host.RunAsync();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return new HostBuilder()
            .UseDasEnvironment()
            .ConfigureDasAppConfiguration(args)
            .UseConsoleLifetime()
            .ConfigureLogging((context, loggingBuilder) =>
            {
                var appInsightsKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
                if (!string.IsNullOrEmpty(appInsightsKey))
                {
                    loggingBuilder.AddNLog(context.HostingEnvironment.IsDevelopment() ? "nlog.development.config" : "nlog.config");
                    loggingBuilder.AddApplicationInsightsWebJobs(o => o.InstrumentationKey = appInsightsKey);
                }
            }).ConfigureServices((context, services) =>
            {
                services.AddMemoryCache();
            })
            .UseStructureMap()
            .ConfigureContainer<Registry>(IoC.Initialize);
    }
}