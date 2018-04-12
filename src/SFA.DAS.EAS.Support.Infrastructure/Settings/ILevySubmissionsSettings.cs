using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.Infrastructure.Settings
{
    public interface ILevySubmissionsSettings
    {
        LevySubmissionsApiConfiguration LevySubmissionsApiConfig { get; set; }

        HmrcApiBaseUrlConfig HmrcApiBaseUrlSetting { get; set; }

    }

}
