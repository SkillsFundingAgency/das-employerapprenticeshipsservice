using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class ConfirmOrganisationToRemoveViewModel : ViewModelBase
{
    public string Name { get; set; }
    public string HashedAccountLegalEntitytId { get; set; }
    public string HashedAccountId { get; set; }
    public bool CanBeRemoved { get; set; }
    public bool HasSignedAgreement { get; set; }

    [Required]
    public bool? Remove { get; set; }
}