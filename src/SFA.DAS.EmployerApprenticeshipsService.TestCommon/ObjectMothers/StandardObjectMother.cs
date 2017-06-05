using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
{
    public class StandardObjectMother
    {
        public static Standard Create(string title)
        {
            var random = new Random();

            return new Standard
            {
                Code = random.Next(1,9999),
                Level = random.Next(1,5),
                Title = title
            };
        }

        public static StandardsView CreateView(Standard standard)
        {
            return CreateView(new[] {standard});
        }

        public static StandardsView CreateView(IEnumerable<Standard> standards)
        {
            return new StandardsView
            {
                CreationDate = DateTime.Now,
                Standards = new List<Standard> (standards)
            };
        }
    }
}
