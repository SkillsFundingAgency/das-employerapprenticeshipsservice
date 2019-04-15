using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using Environment = SFA.DAS.Configuration.Environment;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Helpers
{
    public static class ConfigurationManager
    {
        public static void Set(string key, string value)
        {
            throw new NotImplementedException();
        }

        public static string Get(string key)
        {
            //need to pass the connection string in here from config - is there a config type we can use?
            //actually we will just inject this into the container for configuration
            var configRepo = new SFA.DAS.Configuration.AzureTableStorage.AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            var config = configRepo.Get("SFA.DAS.EmployerFinance", Environment.Local.ToString(), "1.0");
            return "tbc";
        }
    }
}
