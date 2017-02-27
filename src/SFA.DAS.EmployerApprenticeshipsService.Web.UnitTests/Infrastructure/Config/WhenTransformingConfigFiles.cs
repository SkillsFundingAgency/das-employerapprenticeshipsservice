using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;

namespace SFA.DAS.EAS.Web.UnitTests.Infrastructure.Config
{
    public class WhenTransformingConfigFiles
    {
        private List<string> _configFiles;

        [SetUp]
        public void Arrange()
        {
            var extensions = new[] {".config", ".cscfg"};
            var excludedPaths = new[] { "obj", "bin", "vs", "package", "tool", "test" };

            var path = new FileInfo(Assembly.GetCallingAssembly().Location).Directory.Parent.Parent.Parent;

            _configFiles = Directory.GetFiles(path.FullName, "*.*", SearchOption.AllDirectories)
                .Where(c=> !excludedPaths.Any(p=> Path.GetFullPath(c).ToLower().Contains(p)))
                .Where(c=>extensions.Contains(Path.GetExtension(c)))
                .ToList();
        }
        
        [Test]
        public void ThenTheValuesAreCheckedForSecrets()
        {
            foreach (var configFile in _configFiles)
            {
                var xmlConfig = XDocument.Load(configFile);

                var settings = from p in xmlConfig.Descendants()
                              where p.Name.LocalName == "Setting"
                              select p;

                foreach (var setting in settings)
                {

                    if (setting.Attribute("value").ToString().Equals("UseDevelopmentStorage=true")
                        || string.IsNullOrEmpty(setting.Attribute("value").ToString()))
                    {
                        continue;
                    }

                    Assert.IsTrue(setting.Attribute("value").ToString().Contains("__"));
                }

            }
        }
    }

}
