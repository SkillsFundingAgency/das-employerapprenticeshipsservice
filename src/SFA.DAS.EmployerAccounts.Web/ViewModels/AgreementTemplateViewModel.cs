using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Web.Extensions;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class AgreementTemplateViewModel
{
    public int Id { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string PartialViewName { get; set; }
    public int VersionNumber { get; set; }
    public AgreementType AgreementType { get; set; }        
    public DateTime? PublishedDate { get; set; }
    public string PublishedInfo => PublishedDate.HasValue ? $"Published {PublishedDate.Value.ToGdsFormatFull()}" : "";
}