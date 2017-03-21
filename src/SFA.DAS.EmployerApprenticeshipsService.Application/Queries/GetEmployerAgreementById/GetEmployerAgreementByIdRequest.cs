using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreementById
{
    public class GetEmployerAgreementByIdRequest : IAsyncRequest<GetEmployerAgreementByIdResponse>
    {
        public string HashedAgreementId { get; set; }
    }
}