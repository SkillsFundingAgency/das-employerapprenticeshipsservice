using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.CreateEnglishFractionCalculationDate
{
    public class CreateEnglishFractionCalculationDateCommand : IAsyncRequest
    {
        public DateTime DateCalculated { get; set; }
    }
}