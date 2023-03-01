//using Newtonsoft.Json;
//using Newtonsoft.Json.Schema.Generation;
//using NUnit.Framework;
//using SFA.DAS.EAS.Account.Api.Client;
//using SFA.DAS.EAS.Support.Infrastructure.Settings;
//using SFA.DAS.EAS.Support.Web.Configuration;
//using SFA.DAS.Support.Shared.SiteConnection;
//using SFA.DAS.TokenService.Api.Client;
//using System;
//using System.IO;

//namespace SFA.DAS.EAS.Support.Web.Tests.Configuration
//{
//    [TestFixture]
//    public class WebConfigurationTesting
//    {
//        [SetUp]
//        public void Setup()
//        {
//            _unit = new WebConfiguration
//            {
//                AccountApi = new AccountApiConfiguration
//                {
//                    ApiBaseUrl = "--- configuration value goes here ---",
//                    ClientId = "12312312-140e-4f9f-807b-112312312375",
//                    ClientSecret = "--- configuration value goes here ---",
//                    IdentifierUri = "--- configuration value goes here ---",
//                    Tenant = "--- configuration value goes here ---"
//                },
//                EmployerAccountsConfiguration = new EmployerAccountsConfiguration(),
//                SiteValidator = new SiteValidatorSettings
//                {
//                    Audience = "--- configuration value goes here ---",
//                    Scope = "--- configuration value goes here ---",
//                    Tenant = "--- configuration value goes here ---"
//                },
//                LevySubmission = new LevySubmissionsSettings
//                {
//                    HmrcApi = new HmrcApiClientConfiguration
//                    {
//                        ApiBaseUrl = "--- configuration value goes here ---"
//                    },
//                    TokenServiceApi = new TokenServiceApiClientConfiguration
//                    {
//                        ApiBaseUrl = "",
//                        ClientSecret = ""
//                    }
//                },
//                HashingService = new HashingServiceConfig
//                {
//                    AllowedCharacters = "",
//                    Hashstring = ""
//                }
//            };
//        }

//        private const string SiteConfigFileName = "SFA.DAS.Support.EAS";

//        private WebConfiguration _unit;

//        [Test]
//        public void ItShouldDeserialiseFaithfuly()
//        {
//            var json = JsonConvert.SerializeObject(_unit);
//            Assert.IsNotNull(json);
//            var actual = JsonConvert.DeserializeObject<WebConfiguration>(json);
//            Assert.AreEqual(json, JsonConvert.SerializeObject(actual));
//        }

//        [Test]
//        public void ItShouldDeserialize()
//        {
//            var json = JsonConvert.SerializeObject(_unit);
//            Assert.IsNotNull(json);
//            var actual = JsonConvert.DeserializeObject<WebConfiguration>(json);
//            Assert.IsNotNull(actual);
//        }


//        [Test]
//        public void ItShouldGenerateASchema()
//        {
//            var provider = new FormatSchemaProvider();
//            var jSchemaGenerator = new JSchemaGenerator();
//            jSchemaGenerator.GenerationProviders.Clear();
//            jSchemaGenerator.GenerationProviders.Add(provider);
//            var actual = jSchemaGenerator.Generate(typeof(WebConfiguration));


//            Assert.IsNotNull(actual);
//            // hack to leverage format as 'environmentVariable'
//            var schemaString = actual.ToString().Replace($"\"format\":", "\"environmentVariable\":");
//            Assert.IsNotNull(schemaString);
//            File.WriteAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\{SiteConfigFileName}.schema.json",
//                schemaString);
//        }

//        [Test]
//        public void ItShouldSerialize()
//        {
//            var json = JsonConvert.SerializeObject(_unit);
//            Assert.IsFalse(string.IsNullOrWhiteSpace(json));


//            File.WriteAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\{SiteConfigFileName}.json", json);
//        }
//    }
//}