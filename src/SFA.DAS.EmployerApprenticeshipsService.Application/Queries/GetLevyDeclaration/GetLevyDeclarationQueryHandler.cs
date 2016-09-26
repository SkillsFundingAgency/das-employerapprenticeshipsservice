using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationQueryHandler : IAsyncRequestHandler<GetLevyDeclarationRequest, GetLevyDeclarationResponse>
    {
        private readonly IDasLevyRepository _repository;
        private readonly IValidator<GetLevyDeclarationRequest> _validator;
        private readonly IDasAccountService _dasAccountService;

        public GetLevyDeclarationQueryHandler(IDasLevyRepository repository, IValidator<GetLevyDeclarationRequest> validator, IDasAccountService dasAccountService)
        {
            _repository = repository;
            _validator = validator;
            _dasAccountService = dasAccountService;
        }

        public async Task<GetLevyDeclarationResponse> Handle(GetLevyDeclarationRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var declarations = await _repository.GetAccountLevyDeclarations(message.AccountId);
            var schemeInformation = await _dasAccountService.GetAccountSchemes(message.AccountId);

            return new GetLevyDeclarationResponse
            { 
                Data = new LevyDeclarationSourceData
                {
                    AccountId = message.AccountId,
                    Data = declarations.Select(item =>
                    {
                        var scheme = schemeInformation?.SchemesList?.FirstOrDefault(c => c.Ref.Equals(item.EmpRef, StringComparison.CurrentCultureIgnoreCase));
                        return new LevyDeclarationSourceDataItem
                        {
                            Id = item.Id,
                            EmpRef = item.EmpRef,
                            SubmissionDate = item.SubmissionDate,
                            LevyDueYtd = item.LevyDueYtd,
                            EnglishFraction = item.EnglishFraction,
                            PayrollDate = item.PayrollDate(),
                            PayrollMonth = item.PayrollMonth,
                            PayrollYear = item.PayrollYear,
                            LastSubmission = item.LastSubmission,
                            TopUp = item.TopUp,
                            AccountId = item.AccountId,
                            EmprefAddedDate = scheme?.AddedDate ?? DateTime.UtcNow,
                            EmprefRemovedDate = scheme?.RemovedDate
                        };
                    }).ToList()
                }
            };
        }
        
    }
}