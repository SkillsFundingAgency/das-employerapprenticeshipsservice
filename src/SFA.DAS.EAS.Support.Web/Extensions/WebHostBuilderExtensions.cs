﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EAS.Domain.Configuration;
using Microsoft.AspNetCore.Builder;

namespace SFA.DAS.EAS.Support.Web.Extensions;

public static class WebHostBuilderExtensions
{
    public static IWebHostBuilder ConfigureDasAppConfiguration(this IWebHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureAppConfiguration(c => c
            .AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = new[]
                {
                    ConfigurationKeys.EmployerApprenticeshipsService
                };
                options.PreFixConfigurationKeys = false;
            })
            .AddAzureTableStorage(ConfigurationKeys.EmployerApprenticeshipsService));
    }
}