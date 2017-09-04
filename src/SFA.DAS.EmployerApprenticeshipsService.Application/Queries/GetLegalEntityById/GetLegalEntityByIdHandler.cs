using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntityById
{
    public class GetLegalEntityByIdHandler : IAsyncRequestHandler<GetLegalEntityByIdQuery, GetLegalEntityByIdResponse>
    {
        private readonly IValidator<GetLegalEntityByIdQuery> _validator;
        private readonly ILegalEntityRepository _legalEntityRepository;
        private readonly IHashingService _hashingService;

        public GetLegalEntityByIdHandler(
            IValidator<GetLegalEntityByIdQuery> validator, 
            ILegalEntityRepository legalEntityRepository,
            IHashingService hashingService)
        {
            _validator = validator;
            _legalEntityRepository = legalEntityRepository;
            _hashingService = hashingService;
        }

        public async Task<GetLegalEntityByIdResponse> Handle(GetLegalEntityByIdQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);
            var legalEntity = await _legalEntityRepository.GetLegalEntityById(accountId, message.Id);

            return new GetLegalEntityByIdResponse { LegalEntity = legalEntity};
        }
    }
}
