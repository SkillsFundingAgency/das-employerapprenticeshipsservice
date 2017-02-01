using System;

namespace SFA.DAS.EAS.Domain.Data.Entities.Account
{
    public class PayeSchemeView
    {
        public string Ref { get; set; }
        public string Name { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? RemovedDate { get; set; }
    }
}
