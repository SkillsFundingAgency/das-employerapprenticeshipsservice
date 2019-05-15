using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class EmulatedFundingViewModel
    {
        public Guid Id { get; set; }
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string AccountLegalEntityName { get; set; }
        public string CourseId { get; set; }
        public DateTime StartDate { get; set; }
        public string CourseName { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool PublishEvent { get; set; }
    }
}