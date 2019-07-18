using System;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Domain.Models.Employer
{
    public class EmployerInformation
    {
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        [JsonProperty("company_number")]
        public string CompanyNumber { get; set; }

        [JsonProperty("date_of_creation")]
        public DateTime? DateOfIncorporation { get; set; }

        [JsonProperty("registered_office_address")]
        public Address RegisteredAddress { get; set; }

        [JsonProperty("company_status")]
        public string CompanyStatus { get; set; }
    }

    public class Address
    {
        [JsonProperty("address_line_1")]
        public string Line1 { get; set; }
        [JsonProperty("address_line_2")]
        public string Line2 { get; set; }
        [JsonProperty("locality")]
        public string TownOrCity { get; set; }
        [JsonProperty("region")]
        public string County { get; set; }
        [JsonProperty("postal_code")]
        public string PostCode { get; set; }
    }
}