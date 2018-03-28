using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.FeatureToggles
{
    public class WhiteList
    {
        public WhiteList()
        {
            Emails = new List<string>();
        }
        public List<string> Emails { get; set; }
    }
}