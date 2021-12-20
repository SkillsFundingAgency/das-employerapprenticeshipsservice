using System;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.EmployerAccounts.TestCommon.ObjectMothers
{
    public class ApprenticeshipObjectMother
    {
        public static GetApprenticeshipResponse Create(string firstName, string lastName)
        {
            var random = new Random();

            return new GetApprenticeshipResponse
            {
                Id = random.Next(1, 99999),
                FirstName = firstName,
                LastName = lastName,
                NINumber = $"AB {random.Next(10, 99)} {random.Next(10, 99)} {random.Next(10, 99)} C",
                StartDate = DateTime.Now.AddDays(20)
            };
        }
    }
}
