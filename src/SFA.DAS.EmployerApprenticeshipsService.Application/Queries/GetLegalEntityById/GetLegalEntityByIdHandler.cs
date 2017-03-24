using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntityById
{
    public class GetLegalEntityByIdHandler : IAsyncRequestHandler<GetLegalEntityByIdQuery, GetLegalEntityByIdResponse>
    {
        private readonly IValidator<GetLegalEntityByIdQuery> _validator;
        private readonly ILegalEntityRepository _legalEntityRepository;

        public GetLegalEntityByIdHandler(IValidator<GetLegalEntityByIdQuery> validator, ILegalEntityRepository legalEntityRepository)
        {
            _validator = validator;
            _legalEntityRepository = legalEntityRepository;
        }

        public async Task<GetLegalEntityByIdResponse> Handle(GetLegalEntityByIdQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var legalEntity = await _legalEntityRepository.GetLegalEntityById(message.Id);

            return new GetLegalEntityByIdResponse { LegalEntity = legalEntity};
        }
    }
}
