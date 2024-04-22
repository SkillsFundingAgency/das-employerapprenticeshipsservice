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
        Assert.That(json, Is.Not.Null);
        var actual = JsonConvert.DeserializeObject<EasSupportConfiguration>(json);
        Assert.That(json, Is.EqualTo(JsonConvert.SerializeObject(actual)));
    }

    [Test]
    public void ItShouldDeserialize()
    {
        var json = JsonConvert.SerializeObject(_unit);
        Assert.That(json, Is.Not.Null);
        var actual = JsonConvert.DeserializeObject<EasSupportConfiguration>(json);
        Assert.That(actual, Is.Not.Null);
    }

    [Test]
    public void ItShouldGenerateASchema()
    {
        var provider = new FormatSchemaProvider();
        var jSchemaGenerator = new JSchemaGenerator();
        jSchemaGenerator.GenerationProviders.Clear();
        jSchemaGenerator.GenerationProviders.Add(provider);
        var actual = jSchemaGenerator.Generate(typeof(EasSupportConfiguration));

        Assert.That(actual, Is.Not.Null);
        // hack to leverage format as 'environmentVariable'
        var schemaString = actual.ToString().Replace($"\"format\":", "\"environmentVariable\":");
        Assert.That(schemaString, Is.Not.Null);
        File.WriteAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\{SiteConfigFileName}.schema.json",
            schemaString);
    }

    [Test]
    public void ItShouldSerialize()
    {
        var json = JsonConvert.SerializeObject(_unit);
        Assert.That(string.IsNullOrWhiteSpace(json), Is.False);

        File.WriteAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\{SiteConfigFileName}.json", json);
    }
}