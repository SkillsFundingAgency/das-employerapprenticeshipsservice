using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccountsByDateRange;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetPagedEmployerAccountsByDateRangeTests
{
    public class WhenIValidateTheQuery
    {
        private GetPagedEmployerAccountsByDateRangeValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetPagedEmployerAccountsByDateRangeValidator();
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheFieldsArentPopulated()
        {
            //Act
            var actual =_validator.Validate(new GetPagedEmployerAccountsByDateRangeQuery());

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("PageNumber", "PageNumber has not been supplied"),actual.ValidationDictionary );
            Assert.Contains(new KeyValuePair<string,string>("PageSize", "PageSize has not been supplied"),actual.ValidationDictionary );
        }

        [Test]
        public void ThenTrueIsReturnedWhenAllFieldsHaveBeenPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetPagedEmployerAccountsByDateRangeQuery
                {
                    FromDate = DateTime.MinValue,
                    ToDate = DateTime.MaxValue,
                    PageNumber = 10,
                    PageSize = 2
                });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
