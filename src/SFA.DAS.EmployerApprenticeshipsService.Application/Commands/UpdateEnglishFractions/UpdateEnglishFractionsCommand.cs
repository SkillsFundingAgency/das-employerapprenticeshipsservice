using System;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetEnglishFractionUpdateRequired;

namespace SFA.DAS.EAS.Application.Commands.UpdateEnglishFractions
{
    public class UpdateEnglishFractionsCommand : IAsyncRequest
    {
        public string EmployerReference { get; set; }
        public GetEnglishFractionUpdateRequiredResponse EnglishFractionUpdateResponse { get; set; }
    }
}
