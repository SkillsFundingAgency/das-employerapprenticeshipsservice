using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.EAS.Web.Models
{
    public class RenameEmployerAccountViewModel : ViewModelBase
    {
        public string HashedId { get; set; }
        public string CurrentName { get; set; }
        public string NewName { get; set; }
        public string NewNameError => GetErrorMessage(nameof(NewName));
    }
}