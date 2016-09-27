using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLatestEmployerAgreementTemplate
{
    public class GetLatestEmployerAgreementTemplateRequest : IAsyncRequest<GetLatestEmployerAgreementResponse>
    {
        public string HashedId { get; set; }

        public string UserId { get; set; }
    }
}
