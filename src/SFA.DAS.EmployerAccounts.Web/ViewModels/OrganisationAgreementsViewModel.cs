using System.Collections.ObjectModel;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class OrganisationAgreementsViewModel
{
    public OrganisationAgreementsViewModel()
    {
        Agreements = new Collection<OrganisationAgreementViewModel>();
    }

    public ICollection<OrganisationAgreementViewModel> Agreements { get; set; }

    public string AgreementId { get; set; }

    public bool HasSignedAgreements => Agreements.Any(agreement => agreement.SignedDate != null);
    public bool HasUnsignedAgreement => Agreements.Any(agreement => agreement.SignedDate == null);

    public OrganisationAgreementViewModel UnsignedAgreement => Agreements.Single(agreement => agreement.SignedDate == null);

    public IEnumerable<OrganisationAgreementViewModel> SignedAgreements => Agreements.Where(agreement => agreement.SignedDate != null);
}