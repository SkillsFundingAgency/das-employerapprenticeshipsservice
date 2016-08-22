namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces
{
    public interface IConfiguration
    {
        string DatabaseConnectionString { get; set; }

        string ServiceBusConnectionString { get; set; }
    }
}