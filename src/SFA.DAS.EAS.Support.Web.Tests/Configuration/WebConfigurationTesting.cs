﻿using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema.Generation;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using SFA.DAS.EAS.Support.Web.Configuration;

namespace SFA.DAS.EAS.Support.Web.Tests.Configuration;

[TestFixture]
public class WebConfigurationTesting
{
    [SetUp]
    public void Setup()
    {
        _unit = new EasSupportConfiguration
        {
            AccountApi = new AccountApiConfiguration
            {
                ApiBaseUrl = "--- configuration value goes here ---",
                ClientId = "12312312-140e-4f9f-807b-112312312375",
                ClientSecret = "--- configuration value goes here ---",
                IdentifierUri = "--- configuration value goes here ---",
                Tenant = "--- configuration value goes here ---"
            },
            EmployerAccountsConfiguration = new EmployerAccountsConfiguration(),
            LevySubmission = new LevySubmissionsSettings
            {
                HmrcApi = new HmrcApiClientConfiguration
                {
                    ApiBaseUrl = "--- configuration value goes here ---"
                },
                TokenServiceApi = new TokenServiceApiClientConfiguration
                {
                    ApiBaseUrl = "",
                    ClientSecret = ""
                }
            },
            HashingService = new HashingServiceConfig
            {
                AllowedCharacters = "",
                Hashstring = ""
            },
            SiteValidator = new SiteValidatorSettings
            {
                Audience = "some rubbish",
                Scope = "scopy",
                Tenant = "David"
            }
        };
    }

    private const string SiteConfigFileName = "SFA.DAS.Support.EAS";

    private EasSupportConfiguration? _unit;

    [Test]
    public void ItShouldDeserializeFaithfully()
    {
        var json = JsonConvert.SerializeObject(_unit);
        json.Should().NotBeNull();
        var actual = JsonConvert.DeserializeObject<EasSupportConfiguration>(json);
        json.Should().BeEquivalentTo(JsonConvert.SerializeObject(actual));
    }

    [Test]
    public void ItShouldGenerateASchema()
    {
        var provider = new FormatSchemaProvider();
        var jSchemaGenerator = new JSchemaGenerator();
        jSchemaGenerator.GenerationProviders.Clear();
        jSchemaGenerator.GenerationProviders.Add(provider);
        var actual = jSchemaGenerator.Generate(typeof(EasSupportConfiguration));

        actual.Should().NotBeNull();
        // hack to leverage format as 'environmentVariable'
        var schemaString = actual.ToString().Replace($"\"format\":", "\"environmentVariable\":");
        schemaString.Should().NotBeNull();
        File.WriteAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\{SiteConfigFileName}.schema.json",
            schemaString);
    }

    [Test]
    public void ItShouldSerialize()
    {
        var json = JsonConvert.SerializeObject(_unit);
        json.Should().NotBeEmpty();

        File.WriteAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\{SiteConfigFileName}.json", json);
    }
}