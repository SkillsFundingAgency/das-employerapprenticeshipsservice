using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementsRemove
{
    public class GetAccountEmployerAgreementsRemoveQueryHandler : IAsyncRequestHandler<GetAccountEmployerAgreementsRemoveRequest, GetAccountEmployerAgreementsRemoveResponse>
    {
        private readonly IValidator<GetAccountEmployerAgreementsRemoveRequest> _validator;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;

        public GetAccountEmployerAgreementsRemoveQueryHandler(IValidator<GetAccountEmployerAgreementsRemoveRequest> validator, IEmployerAgreementRepository employerAgreementRepository, IHashingService hashingService)
        {
            _validator = validator;
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
        }

        public async Task<GetAccountEmployerAgreementsRemoveResponse> Handle(GetAccountEmployerAgreementsRemoveRequest message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }
            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            var result = await _employerAgreementRepository.GetEmployerAgreementsToRemove(accountId);

            if (result != null && result.Count == 1)
            {
                result.First().CanBeRemoved = false;
            }

            if(result!= null)
            {
                foreach (var removeEmployerAgreementView in result)
                {
                    removeEmployerAgreementView.HashedAgreementId = _hashingService.HashValue(removeEmployerAgreementView.Id);
                }
            }
            

            return new GetAccountEmployerAgreementsRemoveResponse {Agreements = result };
        }
    }
}