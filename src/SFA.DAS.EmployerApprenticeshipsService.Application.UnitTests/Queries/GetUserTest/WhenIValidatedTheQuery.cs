using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUser;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetUserTest
{
    public class WhenIValidatedTheQuery
    {
        private GetUserQueryValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetUserQueryValidator();
        }

        [Test]
        public void ThenIShouldPassIfIHaveAValudUserId()
        {
            //Act
            var result = _validator.Validate(new GetUserQuery {UserId = 1});

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenIShouldFailIfIDoNotHaveAValudUserId()
        {
            //Act
            var result = _validator.Validate(new GetUserQuery());

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }
}
