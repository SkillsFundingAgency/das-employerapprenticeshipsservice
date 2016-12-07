using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetLatestEmployerAgreementTemplate
{
    public class GetLatestEmployerAgreementTemplateRequest : IAsyncRequest<GetLatestEmployerAgreementResponse>
    {
        public string HashedAccountId { get; set; }

        public string UserId { get; set; }
    }
}
