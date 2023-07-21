using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Models.Organisation;

public class AccountLegalEntityModel
{
    public long AccountLegalEntityId { get; set; }
    public long LegalEntityId { get; set; }
    public string AccountLegalEntityPublicHashedId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Identifier { get; set; }
    public OrganisationType OrganisationType { get; set; }
}