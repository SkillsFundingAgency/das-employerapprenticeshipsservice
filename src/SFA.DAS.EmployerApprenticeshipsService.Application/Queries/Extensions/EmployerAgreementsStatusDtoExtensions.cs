using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SFA.DAS.EAS.Application.Dtos.EmployerAgreement;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.Extensions
{
    public static class EmployerAgreementsStatusDtoExtensions
    {
        public static IEnumerable<EmployerAgreementStatusDto> PostFixEmployerAgreementStatusDto(
            this IEnumerable<EmployerAgreementStatusDto> items,
            IHashingService hashingService,
            long accountId)
        {
            void DoIf(EmployerAgreementStatusDto ea, bool runIf, Action action)
            {
                if (runIf)
                {
                    action();
                }
            }

            return items.SetItemValues(
                ag => ag.AccountId = accountId,
                ag => ag.HashedAccountId = hashingService.HashValue(ag.AccountId),
                ag => DoIf(ag, ag.HasSignedAgreement, () => ag.Signed.HashedAgreementId = hashingService.HashValue(ag.Signed.Id)),
                ag => DoIf(ag, ag.HasPendingAgreement, () => ag.Pending.HashedAgreementId = hashingService.HashValue(ag.Pending.Id)),
                ag => DoIf(ag, ag.HasSignedAgreement && ag.HasPendingAgreement && ag.Signed.VersionNumber > ag.Pending.VersionNumber, () => ag.Pending = null)
            );
        }

        public static IQueryable<EmployerAgreementStatusDto> GetAgreementStatus(
            this IQueryable<EmployerAgreement> query,
            IConfigurationProvider configurationProvider,
            Expression<Func<EmployerAgreement, bool>> predicate)
        {
            return query.Where(predicate)
                .Where(ag => ag.StatusId == EmployerAgreementStatus.Signed ||
                             ag.StatusId == EmployerAgreementStatus.Pending)
                .GroupBy(grp => grp.LegalEntity)
                .ProjectTo<EmployerAgreementStatusDto>(configurationProvider);
        }
    }
}