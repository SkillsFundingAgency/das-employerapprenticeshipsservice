using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;

namespace SFA.DAS.EmployerAccounts.Infrastructure.OuterApiRequests.Reservations
{
    public class GetReservationsRequest : IGetApiRequest
    {
        private readonly long _accountId;
        public string GetUrl => $"reservation/{_accountId}";
        public GetReservationsRequest(long accountId)
        {
            _accountId = accountId;
        }
    }
}
