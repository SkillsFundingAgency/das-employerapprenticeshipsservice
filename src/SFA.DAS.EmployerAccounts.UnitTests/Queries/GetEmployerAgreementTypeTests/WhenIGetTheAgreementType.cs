using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementType;
using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.Testing.EntityFramework;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetEmployerAgreementTypeTests
{
    public class WhenIGetTheAgreementType
    {
        private GetEmployerAgreementTypeQueryHandler _handler;
        private Mock<IHashingService> _hashingService;
        private Mock<IValidator<GetEmployerAgreementTypeRequest>> _validator;
        private Mock<EmployerAccountsDbContext> _db;
        private DbSetStub<EmployerAgreement> _agreementDbSet;
        private EmployerAgreement _agreement;

        [SetUp]
        public void Arrange()
        {
            _hashingService = new Mock<IHashingService>();
            _validator = new Mock<IValidator<GetEmployerAgreementTypeRequest>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAgreementTypeRequest>())).ReturnsAsync(new ValidationResult());
            _db = new Mock<EmployerAccountsDbContext>();

            _agreement = new EmployerAgreement { Id = 1234435, Template = new AgreementTemplate { AgreementType = AgreementType.NonLevyExpressionOfInterest }};
            _agreementDbSet = new DbSetStub<EmployerAgreement>(_agreement);

            _db.Setup(d => d.Agreements).Returns(_agreementDbSet);

            _handler = new GetEmployerAgreementTypeQueryHandler(new Lazy<EmployerAccountsDbContext>(() => _db.Object), _hashingService.Object, _validator.Object);
        }

        [Test]
        public async Task WhenTheRequestIsInvalidThenAValidationExceptionIsThrown()
        {
            var request = new GetEmployerAgreementTypeRequest();
            _validator.Setup(x => x.ValidateAsync(request)).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "A", "B" }}});

            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(request));
        }

        [Test]
        public async Task ThenTheAgreementTypeIsReturned()
        {
            var request = new GetEmployerAgreementTypeRequest { HashedAgreementId = "ABC123" };
            _hashingService.Setup(x => x.DecodeValue(request.HashedAgreementId)).Returns(_agreement.Id);

            var response = await _handler.Handle(request);

            Assert.AreEqual(_agreement.Template.AgreementType, response.AgreementType);
        }
    }
}
