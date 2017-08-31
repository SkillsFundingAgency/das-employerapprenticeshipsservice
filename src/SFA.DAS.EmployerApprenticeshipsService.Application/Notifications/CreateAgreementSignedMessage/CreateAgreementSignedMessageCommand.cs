using MediatR;

namespace SFA.DAS.EAS.Application.Notifications.CreateAgreementSignedMessage
{
    public class CreateAgreementSignedMessageCommand : IAsyncNotification
    {
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
        public long AgreementId { get; set; }
    }
}
