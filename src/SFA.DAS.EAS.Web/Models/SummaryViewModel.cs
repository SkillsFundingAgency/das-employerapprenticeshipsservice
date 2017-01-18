﻿using System;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Web.Models
{
    public class SummaryViewModel
    {
        public OrganisationType OrganisationType { get; set; }
        public short? PublicSectorDataSource { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationReferenceNumber { get; set; }
        public DateTime? OrganisationDateOfInception { get; set; }
        public string RegisteredAddress { get; set; }
        public string PayeReference { get; set; }
        public bool EmpRefNotFound { get; set; }
        public string OrganisationStatus { get; set; }
        public string EmployerRefName { get; set; }
      
    }
}