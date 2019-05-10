using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Types
{
    public class Account
    {
        public Account()
        {
            Organisations = new List<Organisation>();
            SavedStandards = new List<object>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<Organisation> Organisations { get; set; }
        public ICollection<object> SavedStandards { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
