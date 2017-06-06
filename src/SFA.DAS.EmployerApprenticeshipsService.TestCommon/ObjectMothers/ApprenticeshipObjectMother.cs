using System;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
{
    public class ApprenticeshipObjectMother
    {
        public static Apprenticeship Create(string firstName, string lastName)
        {
            var random = new Random();

            return new Apprenticeship
            {
                Id =  random.Next(1, 99999) ,
                FirstName = firstName,
                LastName = lastName,
                NINumber = $"AB {random.Next(10,99)} {random.Next(10, 99)} {random.Next(10, 99)} C",
                StartDate = DateTime.Now.AddDays(20)
            };
        }
    }
}
