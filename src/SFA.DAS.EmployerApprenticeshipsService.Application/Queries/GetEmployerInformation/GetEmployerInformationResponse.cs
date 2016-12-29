using System;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerInformation
{
    public class GetEmployerInformationResponse
    {
        public string CompanyName { get; set; }

        public string CompanyNumber { get; set; }

        public DateTime DateOfIncorporation { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressPostcode { get; set; }
        public string CompanyStatus { get; set; }
    }
}