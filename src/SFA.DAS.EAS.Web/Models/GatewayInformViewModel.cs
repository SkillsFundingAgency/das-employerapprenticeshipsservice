using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.EAS.Web.Models
{
    public class GatewayInformViewModel
    {
        public string ConfirmUrl { get; set; }

        public string BreadcrumbUrl { get; set; }
        public string BreadcrumbDescription { get; set; }
        public bool ValidationFailed { get; set; }
        public bool HideBreadcrumb { get; set; }
    }
}