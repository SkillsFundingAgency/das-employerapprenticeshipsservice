using MediatR;

namespace SFA.DAS.EAS.Application.Commands.SetAccountLegalEntityAgreementStatus
{
    public class SetAccountLegalEntityAgreementStatusCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
    }
}