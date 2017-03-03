using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAgreementPdfTests
{
    public class WhenIValidateTheRequest
    {
        private GetEmployerAgreementPdfValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetEmployerAgreementPdfValidator();
        }

        [Test]
        public void ThenFalseIsReturnedAndTheErrorDictionaryPopulatedWhenTheFieldsArentPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetEmployerAgreementPdfRequest());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("AgreementFileName", "AgreementFileName has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenTrueIsReturnedWhenAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetEmployerAgreementPdfRequest {AgreementFileName = "NewFile.txt"});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

    }
}
