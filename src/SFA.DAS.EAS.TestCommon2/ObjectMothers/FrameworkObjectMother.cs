using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
{
    public class FrameworkObjectMother
    {
        public static Framework Create(string title, string pathwayName)
        {
            var random = new Random();

            var framework =  new Framework
            {
                Title = title,
                Level = random.Next(1, 5),
                FrameworkCode = random.Next(1, 99999),
                ProgrammeType = random.Next(1, 10)
            };

            if (string.IsNullOrEmpty(pathwayName))
                return framework;

            framework.PathwayCode = random.Next(1, 100);
            framework.PathwayName = pathwayName;

            return framework;
        }

        public static FrameworksView CreateView(Framework framework)
        {
            return CreateView(new[] {framework});
        }

        public static FrameworksView CreateView(IEnumerable<Framework> frameworks)
        {
            return new FrameworksView
            {
                CreatedDate = DateTime.Now,
                Frameworks = new List<Framework>(frameworks)
            };
        }
    }
}
