using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementRemove
{
    public class GetAccountEmployerAgreementRemoveQueryHandler : IAsyncRequestHandler<GetAccountEmployerAgreementRemoveRequest, GetAccountEmployerAgreementRemoveResponse>
    {
        private readonly IValidator<GetAccountEmployerAgreementRemoveRequest> _validator;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;

        public GetAccountEmployerAgreementRemoveQueryHandler(IValidator<GetAccountEmployerAgreementRemoveRequest> validator, IEmployerAgreementRepository employerAgreementRepository, IHashingService hashingService)
        {
            _validator = validator;
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
        }

        public async Task<GetAccountEmployerAgreementRemoveResponse> Handle(GetAccountEmployerAgreementRemoveRequest message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }
            if (result.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var agreementId = _hashingService.DecodeValue(message.HashedAgreementId);

            var agreement = await _employerAgreementRepository.GetEmployerAgreement(agreementId);

            if (agreement == null)
            {
                return new GetAccountEmployerAgreementRemoveResponse();
            }

            var agreementView = new RemoveEmployerAgreementView
            {
                CanBeRemoved = true,
                HashedAccountId = message.HashedAccountId,
                HashedAgreementId = message.HashedAgreementId,
                Id = agreement.Id,
                Name = agreement.LegalEntityName
            };

            return new GetAccountEmployerAgreementRemoveResponse {Agreement = agreementView};
        }
    }
}
