using System;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class PayeSchemeViewModel : IAccountResource
    {
        public string DasAccountId { get; set; }
        public string Ref { get; set; }
        public string Name { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? RemovedDate { get; set; }
    }
}