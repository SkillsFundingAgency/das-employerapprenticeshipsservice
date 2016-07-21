using System.Collections.Generic;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration
{
    public class EmployerApprenticeshipsServiceConfiguration
    {
        public CompaniesHouseConfiguration CompaniesHouse { get; set; }
        public EmployerConfiguration Employer { get; set; }
        public AzureServiceBusMessageSubSystemConfiguration ServiceBusConfiguration { get; set; }
    }

    public class EmployerConfiguration
    {
        public string DatabaseConnectionString { get; set; }
    }

    public class CompaniesHouseConfiguration
    {
        public string ApiKey { get; set; }
    }

    public class AzureServiceBusMessageSubSystemConfiguration
    {
        public string ConnectionString { get; set; }
        public List<QueueData> Queues { get; set; }
    }

    public class QueueData
    {
        public string QueueType { get; set; }
        public string QueueName { get; set; }
    }
}