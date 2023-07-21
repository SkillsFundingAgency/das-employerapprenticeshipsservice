using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Models.Account;

public class LegalEntity
{
    public virtual long Id { get; set; }
    public virtual string Code { get; set; }
    public virtual DateTime? DateOfIncorporation { get; set; }        
    public virtual byte? PublicSectorDataSource { get; set; }        
    public virtual string Sector { get; set; }
    public virtual OrganisationType Source { get; set; }
    public virtual string Status { get; set; }
    public virtual ICollection<AccountLegalEntity> AccountLegalEntities { get; set; } = new List<AccountLegalEntity>();

}