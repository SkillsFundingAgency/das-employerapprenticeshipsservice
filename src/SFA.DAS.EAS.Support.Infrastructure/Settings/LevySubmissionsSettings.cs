﻿using Newtonsoft.Json;

namespace SFA.DAS.EAS.Support.Infrastructure.Settings
{
    public class LevySubmissionsSettings : ILevySubmissionsSettings
    {
        [JsonRequired] public TokenServiceApiClientConfiguration TokenServiceApi { get; set; }

        [JsonRequired] public HmrcApiClientConfiguration HmrcApi { get; set; }
    }

   
}
