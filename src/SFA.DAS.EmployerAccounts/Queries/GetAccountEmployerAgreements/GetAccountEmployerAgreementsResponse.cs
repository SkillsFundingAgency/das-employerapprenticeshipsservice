using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;

public class GetAccountEmployerAgreementsResponse
{
    public List<EmployerAgreementStatusDto> EmployerAgreements { get; set; }

    public bool HasPendingAgreements =>
        EmployerAgreements != null && EmployerAgreements.Any(ag => ag.HasPendingAgreement);

    public EmployerAgreementStatusDto TryGetSinglePendingAgreement()
    {
        var onlyPendingAgreement = EmployerAgreements?.Where(x => x.HasPendingAgreement)
            .Take(2)
            .ToArray();

        if (onlyPendingAgreement?.Length == 1)
        {
            return onlyPendingAgreement[0];
        }

        return null;
    }
}