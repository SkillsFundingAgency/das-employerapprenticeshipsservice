using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesQueryHandler : IAsyncRequestHandler<GetAccountLegalEntitiesRequest, GetAccountLegalEntitiesResponse>
    {
        private readonly IValidator<GetAccountLegalEntitiesRequest> _validator;

        public GetAccountLegalEntitiesQueryHandler(IValidator<GetAccountLegalEntitiesRequest> validator)
        {
            _validator = validator;
        }

        public async Task<GetAccountLegalEntitiesResponse> Handle(GetAccountLegalEntitiesRequest message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }
            //TODO call account repository to make sure we are part of the account

            //TODO call repository
            return new GetAccountLegalEntitiesResponse {Entites = new LegalEntities {LegalEntityList = new List<LegalEntity> {new LegalEntity {Id=1,Name="My Test Company"} } } };
        }
    }
}
