using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.UpdateEnglishFractions
{
    public class UpdateEnglishFractionsCommand : IAsyncRequest
    {
        public string EmployerReference { get; set; }
        public string AuthToken { get; set; }
        public DateTime DateCalculated { get; set; }
    }
}
