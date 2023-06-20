using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class CreateNewLegalEntityViewModel
{
    public string HashedAccountId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Address { get; set; }
    public DateTime? IncorporatedDate { get; set; }
    public string ExternalUserId { get; set; }
    public string LegalEntityStatus { get; set; }
    public OrganisationType Source { get; set; }
    public byte? PublicSectorDataSource { get; set; }
    public string Sector { get; set; }
}