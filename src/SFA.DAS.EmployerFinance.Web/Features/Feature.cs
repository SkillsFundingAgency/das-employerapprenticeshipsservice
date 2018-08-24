﻿namespace SFA.DAS.EmployerFinance.Models.Features
{
    public class Feature
    {
        public bool Enabled { get; set; }
        public int? EnabledByAgreementVersion { get; set; }
        public FeatureType FeatureType { get; set; }
        public string[] Whitelist { get; set; }
    }
}
