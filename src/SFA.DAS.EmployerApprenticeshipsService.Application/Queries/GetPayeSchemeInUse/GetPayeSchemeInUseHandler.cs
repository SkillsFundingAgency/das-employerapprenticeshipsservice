using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.GetPayeSchemeInUse
{
    public class GetPayeSchemeInUseHandler : IAsyncRequestHandler<GetPayeSchemeInUseQuery,GetPayeSchemeInUseResponse>
    {
        private readonly IValidator<GetPayeSchemeInUseQuery> _validator;
        private readonly IEmployerSchemesRepository _employerSchemesRepository;

        public GetPayeSchemeInUseHandler(IValidator<GetPayeSchemeInUseQuery> validator, IEmployerSchemesRepository employerSchemesRepository)
        {
            _validator = validator;
            _employerSchemesRepository = employerSchemesRepository;
        }

        public async Task<GetPayeSchemeInUseResponse> Handle(GetPayeSchemeInUseQuery message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var scheme = await _employerSchemesRepository.GetSchemeByRef(message.Empref);

            return new GetPayeSchemeInUseResponse {PayeScheme = scheme};
        }
    }
}