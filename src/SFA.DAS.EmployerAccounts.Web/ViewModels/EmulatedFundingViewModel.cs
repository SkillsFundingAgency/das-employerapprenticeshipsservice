using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class EmulatedFundingViewModel
    {
        public string HashedAccountId { get; set; }
        public string CourseCode { get; set; }
        public string ApprenticeName { get; set; }
        public string CourseName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long ReservationId { get; set; }
        public bool PublishEvent { get; set; }
    }
}