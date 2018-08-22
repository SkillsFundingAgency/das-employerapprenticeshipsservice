﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Configuration
{
    public class GoogleAnalyticsSnippets
    {
        public GoogleAnalyticsDetails GoogleAnalyticsValues { get; set; }

        public class GoogleAnalyticsDetails
        {
            public string GoogleHeaderUrl { get; set; }
            public string GoogleBodyUrl { get; set; }
        }
    }
}
