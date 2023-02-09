using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Extensions;

public static class EmployerAgreementsStatusDtoExtensions
{
    public static IEnumerable<EmployerAgreementStatusDto> PostFixEmployerAgreementStatusDto(
        this IEnumerable<EmployerAgreementStatusDto> items,
        IEncodingService encodingService,
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
            ag => ag.HashedAccountId = encodingService.Encode(ag.AccountId, EncodingType.AccountId),
            ag => DoIf(ag.HasSignedAgreement, () => ag.Signed.HashedAgreementId = encodingService.Encode(ag.Signed.Id, EncodingType.AccountId)),
            ag => DoIf(ag.HasPendingAgreement, () => ag.Pending.HashedAgreementId = encodingService.Encode(ag.Pending.Id, EncodingType.AccountId)),
            ag => DoIf(ag.HasSignedAgreement && ag.HasPendingAgreement && ag.Signed.VersionNumber > ag.Pending.VersionNumber, () => ag.Pending = null)
        );
    }
}