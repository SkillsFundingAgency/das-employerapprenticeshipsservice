using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class Scheme
    {
        public int Id { get; set; }
        public string Ref { get; set; }
        public long AccountId { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime AddedDate { get; set; }
        public DateTime? RemovedDate { get; set; }
    }
}
