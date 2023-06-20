using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Dtos;

public class AccountSpecificLegalEntityDto
{
    public long Id { get; set; }
    public long AccountLegalEntityId { get; set; }
    public string AccountLegalEntityPublicHashedId { get; set; }
    public string Code { get; set; }
    public DateTime? DateOfIncorporation { get; set; }
    public string Name { get; set; }
    public byte? PublicSectorDataSource { get; set; }
    public string RegisteredAddress { get; set; }
    public string Sector { get; set; }
    public OrganisationType Source { get; set; }
    public string Status { get; set; }
}