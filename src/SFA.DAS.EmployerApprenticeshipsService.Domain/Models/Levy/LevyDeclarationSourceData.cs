using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.Levy
{
    public class LevyDeclarationSourceData
    {
        public long AccountId { get; set; }  

        public List<LevyDeclarationSourceDataItem> Data { get; set; }
    }
}