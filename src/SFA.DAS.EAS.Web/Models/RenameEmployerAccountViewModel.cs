using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Models
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