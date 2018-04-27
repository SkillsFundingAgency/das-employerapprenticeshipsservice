using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.Infrastructure.Settings
{
    public class LevySubmissionsSettings: ILevySubmissionsSettings
    {
        [JsonRequired] public LevySubmissionsApiConfiguration LevySubmissionsApiConfig { get; set; }

        [JsonRequired] public HmrcApiBaseUrlConfig HmrcApiBaseUrlSetting { get; set; }
    }

   
}
