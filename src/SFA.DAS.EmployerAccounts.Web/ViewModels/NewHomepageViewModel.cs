using SFA.DAS.EAS.Portal.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class NewHomepageViewModel
    {
        public Account Account { get; set; }
        public FlagsClass Flags { get; set; }


        public class FlagsClass
        {
            public bool AgreementsToSign { get; set; }
        }
    }
}