using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Application.Commands.UpdateOrganisationDetails
{
    public class UpdateOrganisationDetailsHandler : IAsyncRequestHandler<UpdateOrganisationDetailsRequest, UpdateOrganisationDetailsResponse>
    {
        private readonly IValidator<UpdateOrganisationDetailsRequest> _validator;
        private readonly IAccountRepository _accountRepository;

        public UpdateOrganisationDetailsHandler(IValidator<UpdateOrganisationDetailsRequest> validator, IAccountRepository accountRepository)
        {
            _validator = validator;
            _accountRepository = accountRepository;
        }

        public async Task<UpdateOrganisationDetailsResponse> Handle(UpdateOrganisationDetailsRequest request)
        {
            var validationResults = _validator.Validate(request);

            if (!validationResults.IsValid())
            {
                throw new InvalidRequestException(validationResults.ValidationDictionary);
            }

            await _accountRepository.UpdateLegalEntityDetailsForAccount(
                request.AccountLegalEntityId, 
                request.Name, 
                request.Address);

            return new UpdateOrganisationDetailsResponse();
        }
    }
}
