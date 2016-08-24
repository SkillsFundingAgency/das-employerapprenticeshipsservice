using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationQueryHandler : IAsyncRequestHandler<GetLevyDeclarationRequest, GetLevyDeclarationResponse>
    {
        private readonly IDasLevyRepository _repository;
        private readonly IValidator<GetLevyDeclarationRequest> _validator;

        public GetLevyDeclarationQueryHandler(IDasLevyRepository repository, IValidator<GetLevyDeclarationRequest> validator )
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GetLevyDeclarationResponse> Handle(GetLevyDeclarationRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var declarations = await _repository.GetAccountLevyDeclarations(message.AccountId);

            return new GetLevyDeclarationResponse
            { 
                Data = new LevyDeclarationSourceData
                {
                    AccountId = message.AccountId,
                    Data = declarations.Select(item => new LevyDeclarationSourceDataItem
                    {
                        Id = item.Id,
                        EmpRef = item.EmpRef,
                        SubmissionDate = item.SubmissionDate,
                        LevyDueYtd = item.LevyDueYtd,
                        EnglishFraction = item.EnglishFraction,
                        PayrollDate = item.PayrollDate()
                    }).ToList()
                }
            };
        }
        
    }
}