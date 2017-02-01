using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetHMRCLevyDeclaration
{
    public class GetHMRCLevyDeclarationQueryHandler : IAsyncRequestHandler<GetHMRCLevyDeclarationQuery,GetHMRCLevyDeclarationResponse>
    {
        private readonly IValidator<GetHMRCLevyDeclarationQuery> _validator;
        private readonly IHmrcService _hmrcService;

        public GetHMRCLevyDeclarationQueryHandler(IValidator<GetHMRCLevyDeclarationQuery> validator, IHmrcService hmrcService)
        {
            _validator = validator;
            _hmrcService = hmrcService;
        }

        public async Task<GetHMRCLevyDeclarationResponse> Handle(GetHMRCLevyDeclarationQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var declarations = await _hmrcService.GetLevyDeclarations(message.EmpRef);
            
            var getLevyDeclarationResponse = new GetHMRCLevyDeclarationResponse
            {
                LevyDeclarations = declarations,
                Empref = message.EmpRef
            };

            return getLevyDeclarationResponse;
        }
    }
}
