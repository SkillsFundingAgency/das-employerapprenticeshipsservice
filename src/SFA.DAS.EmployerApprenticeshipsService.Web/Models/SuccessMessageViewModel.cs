using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class SuccessMessageViewModel
    {
        public string CompanyName { get; set; }

        public string HeadingMessage { get; set; }
        public string CustomSuccessMessage { get; set; }
    }
}