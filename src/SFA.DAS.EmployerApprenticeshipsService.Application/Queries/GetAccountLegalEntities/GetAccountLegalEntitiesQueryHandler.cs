using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

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

            return new GetAccountLegalEntitiesResponse();
        }
    }
}
