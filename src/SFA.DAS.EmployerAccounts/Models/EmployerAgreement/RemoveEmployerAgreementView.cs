using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

public class RemoveEmployerAgreementView
{
    public string Name { get; set; }
    public EmployerAgreementStatus Status { get; set; }
    public long Id { get; set; }
    public string HashedAgreementId { get; set; }
    public string HashedAccountId { get; set; }
    public bool CanBeRemoved { get; set; }
    public string LegalEntityCode { get; set; }

    public OrganisationType LegalEntitySource { get; set; }
}