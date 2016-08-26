using System;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEnglishFractionUpdateRequired
{
    public class GetEnglishFractionsUpdateRequiredQueryHandler : IAsyncRequestHandler<GetEnglishFractionUpdateRequiredRequest, GetEnglishFractionUpdateRequiredResponse>
    {
        public Task<GetEnglishFractionUpdateRequiredResponse> Handle(GetEnglishFractionUpdateRequiredRequest message)
        {
            throw new NotImplementedException();
        }
    }
}
