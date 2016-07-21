using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class Invitation
    {
        public long Id { get; set; }
        public string AccountName { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}