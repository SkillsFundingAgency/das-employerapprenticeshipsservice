using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.WhileList
{
    public class UserWhiteListLookUp
    {
        public IEnumerable<string> EmailPatterns { get; set; }
    }
}
