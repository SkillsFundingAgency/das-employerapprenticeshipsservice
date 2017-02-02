using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.EAS.Web.Models
{
    public class DeleteApprenticeshipConfirmationViewModel
    {
        public long EmployerAccountId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string HashedApprenticeshipId { get; set; }
        public bool? DeleteConfirmed { get; set; }
        public string ApprenticeshipName { get; set; }
        public string DateOfBirth { get; set; }
    }
}