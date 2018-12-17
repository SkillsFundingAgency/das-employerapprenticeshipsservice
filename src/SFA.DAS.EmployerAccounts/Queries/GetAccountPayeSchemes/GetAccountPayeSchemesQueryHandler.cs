using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes
{
    public class GetAccountPayeSchemesQueryHandler : IAsyncRequestHandler<GetAccountPayeSchemesQuery, GetAccountPayeSchemesResponse>
    {
        private readonly IPayeRepository _payeRepository;
        private readonly IEnglishFractionRepository _englishFractionRepository;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetAccountPayeSchemesQuery> _validator;


        public GetAccountPayeSchemesQueryHandler(
            IPayeRepository payeRepository, 
            IEnglishFractionRepository englishFractionRepository,
            IHashingService hashingService, 
            IValidator<GetAccountPayeSchemesQuery> validator )
        {
            _payeRepository = payeRepository ?? throw new ArgumentNullException(nameof(payeRepository));
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
            
            var payeSchemes = await _payeRepository.GetPayeSchemesByAccountId(accountId);

            if (payeSchemes.Count == 0)
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