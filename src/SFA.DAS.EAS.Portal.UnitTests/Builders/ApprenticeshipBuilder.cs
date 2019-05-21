using SFA.DAS.EAS.Portal.Client.Types;
using System;

namespace SFA.DAS.EAS.Portal.UnitTests.Builders
{
    public class ApprenticeshipBuilder
    {
        private long _id;
        private string _firstName = Guid.NewGuid().ToString();
        private string _lastName = Guid.NewGuid().ToString();
        private string _courseName = Guid.NewGuid().ToString();
        private decimal? _proposedCost;
        private DateTime? _startDate;
        private DateTime? _endDate;

        public ApprenticeshipBuilder()
        {
            var random = new Random();
            _id = random.Next(100, 999);
        }

        public Apprenticeship Build()
        {
            return new Apprenticeship
            {   
                Id = _id,
                FirstName = _firstName,
                LastName = _lastName,
                ProposedCost = _proposedCost,
                CourseName  = _courseName,
                StartDate = _startDate,
                EndDate = _endDate
            };
        }

        public ApprenticeshipBuilder WithId(long id)
        {
            _id = id;
            return this;
        }

        public ApprenticeshipBuilder WithFirstName(string firstName)
        {
            _firstName = firstName;
            return this;
        }

        public ApprenticeshipBuilder WithLastName(string lastName)
        {
            _lastName = lastName;
            return this;
        }

        public ApprenticeshipBuilder WithCourseName(string courseName)
        {
            _courseName = courseName;
            return this;
        }
        
        public ApprenticeshipBuilder WithCost(decimal? proposedCost)
        {
            _proposedCost = proposedCost;
            return this;
        }

        public ApprenticeshipBuilder WithStartDate(DateTime? startDate)
        {
            _startDate = startDate;
            return this;
        }

        public ApprenticeshipBuilder WithEndDate(DateTime? endDate)
        {
            _endDate = endDate;
            return this;
        }
        
        public static implicit operator Apprenticeship(ApprenticeshipBuilder instance)
        {
            return instance.Build();
        }
    }
}
