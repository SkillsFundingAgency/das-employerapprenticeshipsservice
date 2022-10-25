using SFA.DAS.Http.Configuration;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IReservationsClientApiConfiguration : IManagedIdentityClientConfiguration
    {
        bool UseStub { get; set; }
    }
}
