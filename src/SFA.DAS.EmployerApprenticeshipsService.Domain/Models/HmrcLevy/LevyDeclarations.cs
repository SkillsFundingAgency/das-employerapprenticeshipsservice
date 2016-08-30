using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy
{
    public class LevyDeclarations
    {
        [JsonProperty("empref")]
        public string EmpRef { get; set; }

        [JsonProperty("declarations")]
        public List<Declaration> Declarations { get; set; }
    }

    public class PayrollPeriod
    {
        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("month")]
        public short Month { get; set; }
    }

    public class Declaration
    {
        [JsonProperty("id")]
        public string Id { get; set; }
       
        [JsonProperty("dateCeased")]
        public DateTime DateCeased { get; set; }

        [JsonProperty("inactiveFrom")]
        public DateTime InactiveFrom { get; set; }

        [JsonProperty("inactiveTo")]
        public DateTime InactiveTo { get; set; }

        [JsonProperty("noPaymentForPeriod")]
        public bool NoPaymentForPeriod { get; set; }

        [JsonProperty("submissionTime")]
        public string SubmissionTime { get; set; }

        [JsonProperty("payrollPeriod")]
        public PayrollPeriod PayrollPeriod { get; set; }

        [JsonProperty("levyDueYTD")]
        public decimal LevyDueYearToDate { get; set; }

        [JsonProperty("levyAllowanceForFullYear")]
        public decimal LevyAllowanceForFullYear { get; set; }
    }

}