using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHMRCLevyDeclaration
{
    public class GetHMRCLevyDeclarationQueryHandler : IAsyncRequestHandler<GetHMRCLevyDeclarationQuery,GetHMRCLevyDeclarationResponse>
    {
        private readonly IValidator<GetHMRCLevyDeclarationQuery> _validator;
        private readonly ILevyDeclarationService _levyDeclarationService;

        public GetHMRCLevyDeclarationQueryHandler(IValidator<GetHMRCLevyDeclarationQuery> validator, ILevyDeclarationService levyDeclarationService)
        {
            _validator = validator;
            _levyDeclarationService = levyDeclarationService;
        }

        public async Task<GetHMRCLevyDeclarationResponse> Handle(GetHMRCLevyDeclarationQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var declarations = await _levyDeclarationService.GetLevyDeclarations(message.Id);

            var fractions = await _levyDeclarationService.GetEnglishFraction(message.Id);

            var getLevyDeclarationResponse = new GetHMRCLevyDeclarationResponse
            {
                Fractions = fractions,
                Declarations = declarations,
                Empref = message.Id
            };

            return getLevyDeclarationResponse;
        }
    }
}
