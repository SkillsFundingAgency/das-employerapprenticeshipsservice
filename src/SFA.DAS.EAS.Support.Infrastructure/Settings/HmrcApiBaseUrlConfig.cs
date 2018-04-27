using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.Infrastructure.Settings
{
    public class HmrcApiBaseUrlConfig: IHmrcApiBaseUrlConfig
    {
        [JsonRequired]
        public string HmrcApiBaseUrl { get; set; }
    }
}
