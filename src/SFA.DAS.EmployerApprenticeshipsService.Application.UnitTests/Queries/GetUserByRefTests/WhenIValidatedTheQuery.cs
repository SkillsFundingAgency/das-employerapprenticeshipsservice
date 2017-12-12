using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUserByRef;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetUserByRefTests
{
    public class WhenIValidatedTheQuery
    {
        private GetUserByRefQueryValidator _queryValidator;

        [SetUp]
        public void Arrange()
        {
            _queryValidator = new GetUserByRefQueryValidator();
        }

        [Test]
        public void ThenIShouldPassIfIHaveAValudUserRef()
        {
            //Act
            var result = _queryValidator.Validate(new GetUserByRefQuery {UserRef = "123"});

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenIShouldFailIfIDoNotHaveAValudUserId()
        {
            //Act
            var result = _queryValidator.Validate(new GetUserByRefQuery());

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }
}
