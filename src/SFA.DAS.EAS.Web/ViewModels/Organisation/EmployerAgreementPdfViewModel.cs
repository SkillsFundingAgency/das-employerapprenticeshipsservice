using System.IO;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf;

namespace SFA.DAS.EAS.Web.ViewModels.Organisation
{
    public class EmployerAgreementPdfViewModel : ViewModelBase
    {
        public Stream PdfStream { get; set; }
    }
}