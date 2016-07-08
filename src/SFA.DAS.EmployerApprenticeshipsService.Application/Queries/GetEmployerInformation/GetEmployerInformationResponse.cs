using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerInformation
{
    public class GetEmployerInformationResponse
    {
        public string CompanyName { get; set; }

        public string CompanyNumber { get; set; }

        public DateTime DateOfIncorporation { get; set; }
    }
}