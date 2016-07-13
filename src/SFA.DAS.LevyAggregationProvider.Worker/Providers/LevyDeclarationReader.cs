using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.LevyAggregationProvider.Worker.Model;

namespace SFA.DAS.LevyAggregationProvider.Worker.Providers
{
    public class LevyDeclarationReader : ILevyDeclarationReader
    {
        public async Task<SourceData> GetData(string empRef)
        {
            //Get Account that empRef is associated with

            //Get data for the Account

            return new SourceData();
        }
    }
}