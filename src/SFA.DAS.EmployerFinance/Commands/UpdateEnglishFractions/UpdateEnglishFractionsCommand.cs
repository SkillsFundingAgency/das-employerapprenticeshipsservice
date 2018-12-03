using MediatR;
using SFA.DAS.EmployerFinance.Queries.GetEnglishFractionsUpdateRequired;

namespace SFA.DAS.EmployerFinance.Commands.UpdateEnglishFractions
{
    public class UpdateEnglishFractionsCommand : IAsyncRequest
    {
        public string EmployerReference { get; set; }
        public GetEnglishFractionUpdateRequiredResponse EnglishFractionUpdateResponse { get; set; }
    }
}
