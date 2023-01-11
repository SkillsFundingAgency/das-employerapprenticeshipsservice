using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Extensions;

public static class EmployerAgreementsStatusDtoExtensions
{
    public static IEnumerable<EmployerAgreementStatusDto> PostFixEmployerAgreementStatusDto(
        this IEnumerable<EmployerAgreementStatusDto> items,
        IHashingService hashingService,
        long accountId)
    {
        void DoIf(bool runIf, Action action)
        {
            if (runIf)
            {
                action();
            }
        }

        return items.SetItemValues(
            ag => ag.AccountId = accountId,
            ag => ag.HashedAccountId = hashingService.HashValue(ag.AccountId),
            ag => DoIf(ag.HasSignedAgreement, () => ag.Signed.HashedAgreementId = hashingService.HashValue(ag.Signed.Id)),
            ag => DoIf(ag.HasPendingAgreement, () => ag.Pending.HashedAgreementId = hashingService.HashValue(ag.Pending.Id)),
            ag => DoIf(ag.HasSignedAgreement && ag.HasPendingAgreement && ag.Signed.VersionNumber > ag.Pending.VersionNumber, () => ag.Pending = null)
        );
    }
}