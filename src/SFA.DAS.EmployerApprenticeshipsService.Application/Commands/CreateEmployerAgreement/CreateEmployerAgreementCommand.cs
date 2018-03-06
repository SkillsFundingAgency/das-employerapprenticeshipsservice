using MediatR;

namespace SFA.DAS.EAS.Application.Commands.CreateEmployerAgreement
{
    public class CreateEmployerAgreementCommand : IAsyncRequest
    {
        public int LatestTemplateId { get; set; }
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
    }
}