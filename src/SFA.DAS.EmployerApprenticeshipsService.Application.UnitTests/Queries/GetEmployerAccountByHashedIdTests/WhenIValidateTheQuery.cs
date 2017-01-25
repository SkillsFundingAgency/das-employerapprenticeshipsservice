using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountByHashedId;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAccountByHashedIdTests
{
    public class WhenIValidateTheQuery
    {
        private GetEmployerAccountByHashedIdValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetEmployerAccountByHashedIdValidator();
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheHashedIdIsntPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetEmployerAccountByHashedIdQuery());

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("HashedAccountId", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenTrueIsReturnedWhenTheHashedIdHasBeenPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetEmployerAccountByHashedIdQuery
            {
                HashedAccountId = "ABC123"
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
