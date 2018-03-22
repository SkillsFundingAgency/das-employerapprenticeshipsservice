using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetAccountPayeSchemes
{
    public class GetAccountPayeSchemesQueryHandler : IAsyncRequestHandler<GetAccountPayeSchemesQuery, GetAccountPayeSchemesResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEnglishFractionRepository _englishFractionRepository;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetAccountPayeSchemesQuery> _validator;


        public GetAccountPayeSchemesQueryHandler(
            IAccountRepository accountRepository, 
            IEnglishFractionRepository englishFractionRepository,
            IHashingService hashingService, 
            IValidator<GetAccountPayeSchemesQuery> validator )
        {
            if (accountRepository == null)
                throw new ArgumentNullException(nameof(accountRepository));
            _accountRepository = accountRepository;
            _englishFractionRepository = englishFractionRepository;
            _hashingService = hashingService;
            _validator = validator;
        }

        public async Task<GetAccountPayeSchemesResponse> Handle(GetAccountPayeSchemesQuery message)
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
            
            var payeSchemes = await _accountRepository.GetPayeSchemesByAccountId(accountId);

            if (!payeSchemes.Any())
            {
                return new GetAccountPayeSchemesResponse
                {
                    PayeSchemes = payeSchemes
                };
            }

            var englishFractions = (await _englishFractionRepository.GetCurrentFractionForSchemes(accountId, payeSchemes.Select(x => x.Ref))).Where(x => x != null).ToList();
            foreach (var scheme in payeSchemes)
            {
                scheme.EnglishFraction = englishFractions.FirstOrDefault(x => x.EmpRef == scheme.Ref);
            }

            return new GetAccountPayeSchemesResponse
            {
                PayeSchemes = payeSchemes
            };
        }
    }
}