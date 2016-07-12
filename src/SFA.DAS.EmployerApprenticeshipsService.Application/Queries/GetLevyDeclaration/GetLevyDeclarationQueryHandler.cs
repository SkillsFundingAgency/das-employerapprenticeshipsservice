using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationQueryHandler : IAsyncRequestHandler<GetLevyDeclarationQuery,GetLevyDeclarationResponse>
    {
        private readonly IValidator<GetLevyDeclarationQuery> _validator;
        private readonly ILevyDeclarationService _levyDeclarationService;

        public GetLevyDeclarationQueryHandler(IValidator<GetLevyDeclarationQuery> validator, ILevyDeclarationService levyDeclarationService)
        {
            _validator = validator;
            _levyDeclarationService = levyDeclarationService;
        }

        public async Task<GetLevyDeclarationResponse> Handle(GetLevyDeclarationQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var declarations = await _levyDeclarationService.GetLevyDeclarations(message.Id);

            var fractions = await _levyDeclarationService.GetEnglishFraction(message.Id);

            var getLevyDeclarationResponse = new GetLevyDeclarationResponse
            {
                Fractions = fractions,
                Declarations = declarations,
                Empref = message.Id
            };

            return getLevyDeclarationResponse;
        }
    }
}
