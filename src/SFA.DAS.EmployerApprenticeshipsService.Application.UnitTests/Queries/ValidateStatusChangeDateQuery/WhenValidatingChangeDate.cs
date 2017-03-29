using NUnit.Framework;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.ValidateStatusChangeDateQuery
{
    using FluentAssertions;
    using MediatR;
    using Moq;
    using SFA.DAS.Commitments.Api.Types.Apprenticeship;
    using SFA.DAS.EAS.Application.Queries.GetApprenticeship;
    using SFA.DAS.EAS.Application.Queries.ValidateStatusChangeDate;
    using SFA.DAS.EAS.Domain.Interfaces;
    using SFA.DAS.EAS.Domain.Models.Apprenticeship;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    public sealed class WhenValidatingChangeDate
    {
        private ValidateStatusChangeDateQuery _testQuery;
        private ValidateStatusChangeDateQueryHandler _handler;
        private Mock<IMediator> _mockMediator;
        private Mock<ICurrentDateTime> _mockCurrentDate;
        private Apprenticeship _apprenticeship;

        [SetUp]
        public void Setup()
        {
            _testQuery = new ValidateStatusChangeDateQuery { AccountId = 123, ApprenticeshipId = 456, ChangeOption = ChangeOption.SpecificDate };
            _mockCurrentDate = new Mock<ICurrentDateTime>();
            _mockCurrentDate.SetupGet(x => x.Now).Returns(DateTime.UtcNow.AddDays(-5)); // Started training
            _apprenticeship = new Apprenticeship { StartDate = DateTime.UtcNow.Date };

            _mockMediator = new Mock<IMediator>();
            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse { Apprenticeship = _apprenticeship });

            _handler = new ValidateStatusChangeDateQueryHandler(new ValidateStatusChangeDateQueryValidator(), _mockMediator.Object, _mockCurrentDate.Object);
        }

        [Test]
        public async Task WhenDateIsInTheFutureAnValidationErrorReturned()
        {
            _testQuery.DateOfChange = DateTime.UtcNow.AddMonths(2);

            var response = await _handler.Handle(_testQuery);

            response.ValidationResult.IsValid().Should().BeFalse();
            response.ValidationResult.ValidationDictionary.Should().ContainValue("Date must be a date in the past");
        }

        [Test]
        public async Task WhenDateIsEarlierThanTrainingStartDateAnValidationErrorReturned()
        {
            _testQuery.DateOfChange = DateTime.UtcNow.AddMonths(-2);
            _apprenticeship.StartDate = DateTime.UtcNow.Date.AddMonths(-1);

            var response = await _handler.Handle(_testQuery);

            response.ValidationResult.IsValid().Should().BeFalse();
            response.ValidationResult.ValidationDictionary.Should().ContainValue("Date cannot be earlier than training start date");
        }
    }
}
