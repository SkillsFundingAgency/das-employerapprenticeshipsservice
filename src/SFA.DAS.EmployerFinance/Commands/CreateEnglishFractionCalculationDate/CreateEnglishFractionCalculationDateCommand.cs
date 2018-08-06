using System;
using MediatR;

namespace SFA.DAS.EmployerFinance.Commands.CreateEnglishFractionCalculationDate
{
    public class CreateEnglishFractionCalculationDateCommand : IAsyncRequest
    {
        public DateTime DateCalculated { get; set; }
    }
}