using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Models.Account;

public class AccountDetail
{
    public long AccountId { get; set; }
    public string HashedId { get; set; }
    public string PublicHashedId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public string OwnerEmail { get; set; }
    public List<long> LegalEntities { get; set; } = new List<long>();
    public List<string> PayeSchemes { get; set; } = new List<string>();
    public List<AgreementType> AccountAgreementTypes { get; set; }
    public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
}