using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.HashingService;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements
{
    public static class HashingServiceExtensions
    {
        public static string HashIdIfNotNull(this IHashingService hashingService, long? id)
        {
            return id.HasValue ? hashingService.HashValue(id.Value) : null;
        }
    }

    public class GetAccountEmployerAgreementsQueryHandler : IAsyncRequestHandler<GetAccountEmployerAgreementsRequest, GetAccountEmployerAgreementsResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetAccountEmployerAgreementsRequest> _validator;

        public GetAccountEmployerAgreementsQueryHandler(EmployerAccountDbContext db, IHashingService hashingService, IValidator<GetAccountEmployerAgreementsRequest> validator)
        {
            _db = db;
            _hashingService = hashingService;
            _validator = validator;
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
                _db.Agreements
                    // only consider legal entities with either a signed or pending agreement
                    .Where(ag => ag.AccountId == accountId
                                 && (ag.StatusId == EmployerAgreementStatus.Signed ||
                                     ag.StatusId == EmployerAgreementStatus.Pending))
                    // get the highest-version of signed and pending agreement for each legal entity
                    .GroupBy(ag => new {ag.LegalEntity, ag.StatusId})
                    .Select(grp => new
                    {
                        grp.Key.LegalEntity,
                        Status = grp.Key.StatusId,
                        Agreement = grp
                            .OrderByDescending(ag => ag.Template.VersionNumber)
                            .FirstOrDefault()
                    })
                    // get a single result for each legal entity which contains the highest version signed and pending agreements
                    .GroupBy(grp => grp.LegalEntity)
                    .Select(grp => new
                    {
                        LegalEntity = grp.Key,
                        Signed = grp.FirstOrDefault(ag => ag.Status == EmployerAgreementStatus.Signed),
                        Pending = grp.FirstOrDefault(ag => ag.Status == EmployerAgreementStatus.Pending)
                    })

                    // HACK: the following list is needed for the unit test, not for the production code, although it does not break the code. 
                    //      The dbset mocks that are used in the unit tests do not generate SQL (like EF prod does) but instead converts the expressions
                    //      into anonymous delegates. This introduces a subtle change in behaviour; in SQL the properties cannot be null (as they 
                    //      - are actually tuples from nested queries) where as in .net land they can be. So for example, in the expression
                    //      ag.Signed.Agreement signed will never be null in SQL (because it is a projection from a sub query) whereas it will
                    //      be null when operating in .net land. The normal way of dealing with this using null safe propagation cannot be used
                    //      to resolve this because it results in a .net expression which cannot be translated into SQL (or at least isn't translated).
                    //      What to do, what to do??....
                    //      By coercing the expression so far into a list the IQueryable expression is sent to the DB and the projection after the
                    //      ToList() is run purely in .net and so can use the null propagation operator.
                    //      Obviously, adapting prod code to suit the tests is not ideal :-(
                    .ToList()

                    // project into the required shape
                    .Select(ag => new EmployerAgreementStatusView
                    {
                        AccountId = accountId,
                        HashedAccountId = message.HashedAccountId,
                        LegalEntityId = ag.LegalEntity.Id,
                        LegalEntityName = ag.LegalEntity.Name,
                        LegalEntityCode = ag.LegalEntity.Code,
                        LegalEntityAddress = ag.LegalEntity.RegisteredAddress,
                        LegalEntityInceptionDate = ag.LegalEntity.DateOfIncorporation,
                        LegalEntityStatus = ag.LegalEntity.Status,
                        SignedByName = ag.Signed?.Agreement.SignedByName,
                        SignedDate = ag.Signed?.Agreement.SignedDate,
                        SignedExpiredDate = ag.Signed?.Agreement.ExpiredDate,

                        SignedAgreementId = ag.Signed?.Agreement.Id,
                        SignedTemplateId = ag.Signed?.Agreement.TemplateId,
                        SignedTemplatePartialViewName = ag.Signed?.Agreement.Template.PartialViewName,
                        SignedVersion = ag.Signed?.Agreement.Template.VersionNumber,

                        PendingAgreementId = ag.Pending?.Agreement.Id,                                  // <-- we can not use ?. in expressions on IQueryable, so these have to be ienumerable
                        PendingTemplateId = ag.Pending?.Agreement.TemplateId,
                        PendingTemplatePartialViewName = ag.Pending?.Agreement.Template.PartialViewName,
                        PendingVersion = ag.Pending?.Agreement.Template.VersionNumber
                    })
                    .ToList();
                                                    
            foreach (var agreement in agreements)
            {
                agreement.SignedHashedAgreementId = _hashingService.HashIdIfNotNull(agreement.SignedAgreementId);
                agreement.PendingHashedAgreementId = _hashingService.HashIdIfNotNull(agreement.PendingAgreementId);

                // A signed version deprecates any early unsigned agreement.
                if (agreement.HasPendingAgreement && agreement.HasSignedAgreement &&
                    agreement.SignedVersion > agreement.PendingVersion)
                {
                    agreement.PendingVersion = null;
                    agreement.PendingAgreementId = null;
                    agreement.PendingHashedAgreementId = null;
                }
            }

            return new GetAccountEmployerAgreementsResponse
            {
                EmployerAgreements = agreements
            };
        }
    }
}