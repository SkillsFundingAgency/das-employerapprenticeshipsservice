using System;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
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
                StartDate = DateTime.Now.AddDays(20)
            };
        }
    }
}
