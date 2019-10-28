using SFA.DAS.Http;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IReservationsClientApiConfiguration : IAzureADClientConfiguration
    {
        string ApiBaseUrl { get; }
        bool UseStub { get; set; }
    }
}
