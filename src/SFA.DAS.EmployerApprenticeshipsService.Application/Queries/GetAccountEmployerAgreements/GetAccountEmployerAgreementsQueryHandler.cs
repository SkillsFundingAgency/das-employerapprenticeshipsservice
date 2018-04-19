using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.HashingService;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements
{
    public static class HashingServiceExtensions
    {
        public static string HashIdIfNotNull(this IHashingService hashingService, long? id)
        {
            return id.HasValue ? hashingService.HashValue(id.Value) : null;
        }
    }

    public class GetAccountEmployerAgreementsQueryHandlerProjection
    {
        public LegalEntity LegalEntity { get; set; }
        public EmployerAgreement Signed { get; set; }
        public EmployerAgreement Pending { get; set; }
    }

    public class GetAccountEmployerAgreementsQueryHandler : IAsyncRequestHandler<GetAccountEmployerAgreementsRequest, GetAccountEmployerAgreementsResponse>
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

            var agreements =
                await _db.Agreements
                    .Where(ag => ag.AccountId == accountId
                                 && (ag.StatusId == EmployerAgreementStatus.Signed ||
                                     ag.StatusId == EmployerAgreementStatus.Pending))
                    .GroupBy(grp => grp.LegalEntity)
                    .Select(grp => new GetAccountEmployerAgreementsQueryHandlerProjection
                    {
                        LegalEntity = grp.Key,
                        Signed = grp
                            .OrderByDescending(ag => ag.Template.VersionNumber)
                            .FirstOrDefault(ag => ag.StatusId == EmployerAgreementStatus.Signed),
                        Pending = grp
                            .OrderByDescending(ag => ag.Template.VersionNumber)
                            .FirstOrDefault(ag => ag.StatusId == EmployerAgreementStatus.Pending),
                    })
                    .ProjectTo<EmployerAgreementStatusView>(_configurationProvider)
                    .ToListAsync();
                                                    
            foreach (var agreement in agreements)
            {
                agreement.AccountId = accountId;
                agreement.HashedAccountId = message.HashedAccountId;

                if (agreement.HasSignedAgreement)
                {
                    agreement.Signed.HashedAgreementId = _hashingService.HashIdIfNotNull(agreement.Signed.Id);
                }

                if (agreement.HasPendingAgreement)
                {
                    if (agreement.HasSignedAgreement && agreement.Signed.VersionNumber > agreement.Pending.VersionNumber)
                    {
                        agreement.Pending = null;
                    }
                    else
                    {
                        agreement.Pending.HashedAgreementId =
                            _hashingService.HashIdIfNotNull(agreement.Pending.Id);
                    }
                }
            }

            return new GetAccountEmployerAgreementsResponse
            {
                EmployerAgreements = agreements
            };
        }
    }
}