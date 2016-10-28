using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Domain.Models.HmrcLevy
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
        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class FractionCalculation
    {
        [JsonProperty("calculatedAt")]
        public string CalculatedAt { get; set; }

        [JsonProperty("fractions")]
        public List<Fraction> Fractions { get; set; }
    }


}