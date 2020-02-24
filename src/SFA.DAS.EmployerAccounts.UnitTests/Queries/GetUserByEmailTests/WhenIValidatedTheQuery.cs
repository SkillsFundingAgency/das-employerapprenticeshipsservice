using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetUserByEmail;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetUserByEmailTests
{
    public class WhenIValidatedTheQuery
    {
        private GetUserByEmailQueryValidator _queryValidator;

        [SetUp]
        public void Arrange()
        {
            _queryValidator = new GetUserByEmailQueryValidator();
        }

        [Test]
        public void ThenIShouldPassIfIHaveAValudUserRef()
        {
            //Act
            var result = _queryValidator.Validate(new GetUserByEmailQuery {Email = "test@test.com"});

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenIShouldFailIfIDoNotHaveAValudUserId()
        {
            //Act
            var result = _queryValidator.Validate(new GetUserByEmailQuery());

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }
}
