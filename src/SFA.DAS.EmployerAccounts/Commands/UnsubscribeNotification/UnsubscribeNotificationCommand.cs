using MediatR;

namespace SFA.DAS.EmployerAccounts.Commands.UnsubscribeNotification
{
    public class UserIsAuthorizedToSignUnsignedAgreementCommand : IAsyncRequest
    {
        public string UserRef { get; set; }

        public long AccountId { get; set; }
    }
}
