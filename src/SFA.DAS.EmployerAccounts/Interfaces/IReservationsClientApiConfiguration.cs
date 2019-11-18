using SFA.DAS.Http.Configuration;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IReservationsClientApiConfiguration : IAzureActiveDirectoryClientConfiguration
    {
        bool UseStub { get; set; }
    }
}
