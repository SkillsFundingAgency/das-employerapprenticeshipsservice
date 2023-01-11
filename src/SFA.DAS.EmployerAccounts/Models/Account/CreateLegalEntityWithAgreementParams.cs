using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Models.Account;

public class CreateLegalEntityWithAgreementParams
{
    public long AccountId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Address { get; set; }
    public DateTime? DateOfIncorporation { get; set; }
    public string Status { get; set; }
    public OrganisationType Source { get; set; }
    public byte? PublicSectorDataSource { get; set; }
    public string Sector { get; set; }
    public AgreementType AgreementType { get; set; }
}