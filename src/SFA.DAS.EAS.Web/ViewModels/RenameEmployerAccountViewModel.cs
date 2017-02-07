using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class RenameEmployerAccountViewModel : ViewModelBase
    {
        public string HashedId { get; set; }
        [AllowHtml]
        public string CurrentName { get; set; }
        [AllowHtml]
        public string NewName { get; set; }
        public string NewNameError => GetErrorMessage(nameof(NewName));
    }
}