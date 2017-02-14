using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetLastLevyDeclaration;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetHMRCLevyDeclaration
{
    public class GetHMRCLevyDeclarationQueryHandler : IAsyncRequestHandler<GetHMRCLevyDeclarationQuery,GetHMRCLevyDeclarationResponse>
    {
        private readonly IValidator<GetHMRCLevyDeclarationQuery> _validator;
        private readonly IHmrcService _hmrcService;
        private readonly IMediator _mediator;

        public GetHMRCLevyDeclarationQueryHandler(IValidator<GetHMRCLevyDeclarationQuery> validator, IHmrcService hmrcService, IMediator mediator)
        {
            _validator = validator;
            _hmrcService = hmrcService;
            _mediator = mediator;
        }

        public async Task<GetHMRCLevyDeclarationResponse> Handle(GetHMRCLevyDeclarationQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var existingDeclaration = await _mediator.SendAsync(new GetLastLevyDeclarationQuery {EmpRef = message.EmpRef});

            DateTime? dateFrom = null;
            if (existingDeclaration?.Transaction?.Date != null)
            {
                dateFrom = existingDeclaration.Transaction?.Date.AddDays(-1);
            }

            var declarations = await _hmrcService.GetLevyDeclarations(message.EmpRef, dateFrom);
            
            var getLevyDeclarationResponse = new GetHMRCLevyDeclarationResponse
            {
                LevyDeclarations = declarations,
                Empref = message.EmpRef
            };

            return getLevyDeclarationResponse;
        }
    }
}
