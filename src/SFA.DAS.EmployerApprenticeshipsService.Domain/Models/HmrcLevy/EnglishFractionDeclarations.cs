using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy
{
    public class EnglishFractionDeclarations
    {
        [JsonProperty("empref")]
        public string Empref { get; set; }
        [JsonProperty("fractionCalculations")]
        public List<FractionCalculation> FractionCalculations { get; set; }
    }


    public class Fraction
    {
        public string region { get; set; }
        public string value { get; set; }
    }

    public class FractionCalculation
    {
        public string calculatedAt { get; set; }
        public List<Fraction> fractions { get; set; }
    }


}