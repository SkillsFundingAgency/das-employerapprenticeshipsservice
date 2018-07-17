using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.HashingService;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Application.Dtos.EmployerAgreement;
using SFA.DAS.EAS.Application.Queries.Extensions;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements
{
    public class GetAccountEmployerAgreementsQueryHandler : IAsyncRequestHandler<GetAccountEmployerAgreementsRequest,
        GetAccountEmployerAgreementsResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetAccountEmployerAgreementsRequest> _validator;
        private readonly IConfigurationProvider _configurationProvider;


        public GetAccountEmployerAgreementsQueryHandler(
            EmployerAccountDbContext db,
            IHashingService hashingService,
            IValidator<GetAccountEmployerAgreementsRequest> validator,
            IConfigurationProvider configurationProvider
        )
        {
            _db = db;
            _hashingService = hashingService;
            _validator = validator;
            _configurationProvider = configurationProvider;
        }

        public async Task<GetAccountEmployerAgreementsResponse> Handle(GetAccountEmployerAgreementsRequest message)
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

            var agreements = await _db.AccountLegalEntities
                .Where(ale => ale.AccountId == accountId)
                .ProjectTo<EmployerAgreementStatusDto>(_configurationProvider)
                .ToListAsync();
                                    
            agreements = agreements.PostFixEmployerAgreementStatusDto(_hashingService, accountId).ToList();

            return new GetAccountEmployerAgreementsResponse
            {
                EmployerAgreements = agreements
            };
        }
    }
}