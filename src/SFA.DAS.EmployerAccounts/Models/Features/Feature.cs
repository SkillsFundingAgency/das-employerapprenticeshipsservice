using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Models.Features
{
    public class Feature
    {
        public bool Enabled { get; set; }
        public int? EnabledByAgreementVersion { get; set; }
        public FeatureType FeatureType { get; set; }
        public string[] Whitelist { get; set; }
    }
}
