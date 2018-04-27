using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;

namespace SFA.DAS.EAS.Solution.UnitTests.ConfigTests
{
    public class WhenTransformingConfigFiles
    {
        private List<string> _configFiles;
        private string[] _allowedConfigValues;
        private string[] _excludedSettingNames;

        [SetUp]
        public void Arrange()
        {
            var extensions = new[] { ".config", ".cscfg" };
            var excludedPaths = new[] { "obj", "bin", "vs", "package", "tool", "test" };

            _excludedSettingNames = new[] { "ApplicationName", "webpages:Version", "LogLevel", "idaAudience", "idaTenant", "TokenCertificateThumbprint", "DeclarationsEnabled", "CurrentTime", "EmployerCommitmentsBaseUrl" };
            _allowedConfigValues = new[] { "logstash", "UseDevelopmentStorage=true", "LOCAL", "true", "false", "localhost", "Endpoint=sb://[your namespace].servicebus.windows.net;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=[your secret]" };

            var path = new FileInfo(Assembly.GetCallingAssembly().Location).Directory.Parent.Parent.Parent;

            _configFiles = Directory.GetFiles(path.FullName, "*.*", SearchOption.AllDirectories)
                .Where(c => !excludedPaths.Any(p => Path.GetFullPath(c).ToLower().Contains(p)))
                .Where(c => extensions.Contains(Path.GetExtension(c)))
                .ToList();
        }

        [Test]
        public void ThenTheCloudConfigurationValuesAreCheckedForSecrets()
        {
            foreach (var configFile in _configFiles.Where(x => Path.GetExtension(x).Equals(".cscfg")))
            {
                var xmlConfig = XDocument.Load(configFile);

                var settings = from p in xmlConfig.Descendants()
                               where p.Name.LocalName == "Setting"
                               select p;

                foreach (var setting in settings)
                {

                    if (_allowedConfigValues.Any(c => setting.Attribute("value").Value.Contains(c))
                        || _excludedSettingNames.Any(c => setting.Attribute("name").Value.Contains(c))
                        || string.IsNullOrEmpty(setting.Attribute("value").Value))
                    {
                        continue;
                    }

                    Assert.IsTrue(setting.Attribute("value").Value.Contains("__"), $"The setting {setting.Attribute("name")} has a invalid value {setting.Attribute("value")} in {configFile}");
                }

            }
        }

        [Test]
        public void ThenTheConfigurationValuesAreCheckedForSecrets()
        {
            foreach (var configFile in _configFiles.Where(x => Path.GetExtension(x).Equals(".config")))
            {
                var xmlConfig = XDocument.Load(configFile);

                var appSettings = from p in xmlConfig.Descendants()
                                  where p.Name.LocalName == "appSettings"
                                  select p;

                var settings = from p in appSettings.Descendants()
                               where p.Name.LocalName == "add"
                               select p;

                foreach (var setting in settings)
                {
                    if (_allowedConfigValues.Any(c => setting.Attribute("value").Value.Contains(c))
                        || _excludedSettingNames.Any(c => setting.Attribute("key").Value.Contains(c))
                        || string.IsNullOrEmpty(setting.Attribute("value").Value))
                    {
                        continue;
                    }


                    Assert.IsTrue(setting.Attribute("value").Value.Contains("__"), $"The setting {setting.Attribute("key")} has a invalid value {setting.Attribute("value")} in {configFile}");
                }
            }
        }

    }

}
