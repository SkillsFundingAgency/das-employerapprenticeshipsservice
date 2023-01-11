using System.IO;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class EmployerAgreementPdfViewModel : ViewModelBase
{
    public Stream PdfStream { get; set; }
}