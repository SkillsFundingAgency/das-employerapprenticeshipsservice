using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetHMRCLevyDeclaration
{
    public class GetHMRCLevyDeclarationQueryHandler : IAsyncRequestHandler<GetHMRCLevyDeclarationQuery,GetHMRCLevyDeclarationResponse>
    {
        private readonly IValidator<GetHMRCLevyDeclarationQuery> _validator;
        private readonly IHmrcService _hmrcService;
        private readonly IEnglishFractionRepository _englishFractionRepository;

        public GetHMRCLevyDeclarationQueryHandler(IValidator<GetHMRCLevyDeclarationQuery> validator, IHmrcService hmrcService, 
            IEnglishFractionRepository englishFractionRepository)
        {
            _validator = validator;
            _hmrcService = hmrcService;
            _englishFractionRepository = englishFractionRepository;
        }

        public async Task<GetHMRCLevyDeclarationResponse> Handle(GetHMRCLevyDeclarationQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var declarations = await _hmrcService.GetLevyDeclarations(message.AuthToken, message.EmpRef);
            
            var fractions = await _hmrcService.GetEnglishFractions(message.AuthToken, message.EmpRef);
            
            var getLevyDeclarationResponse = new GetHMRCLevyDeclarationResponse
            {
                Fractions = fractions,
                LevyDeclarations = declarations,
                Empref = message.EmpRef
            };

            return getLevyDeclarationResponse;
        }
    }
}
