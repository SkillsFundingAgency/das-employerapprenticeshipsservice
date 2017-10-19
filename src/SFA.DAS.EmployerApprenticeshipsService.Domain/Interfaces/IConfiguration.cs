using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IConfiguration
    {
        string DatabaseConnectionString { get; set; }
        string ServiceBusConnectionString { get; set; }
        string MessageServiceBusConnectionString { get; set; }
    }
}