using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.CreateEnglishFractionCalculationDate
{
    public class CreateEnglishFractionCalculationDateCommandHandler : AsyncRequestHandler<CreateEnglishFractionCalculationDateCommand>
    {
        protected override Task HandleCore(CreateEnglishFractionCalculationDateCommand message)
        {
            throw new NotImplementedException();
        }
    }
}
