using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Api.Controllers;
using SFA.DAS.EAS.Api.Models;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountsByHashedId;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsInformationTests
{
    public class WhenIGetAccountInformationByHashedId
    {
        private EmployerAccountsInformationController _controller;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private AccountInformation _accountInformation;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger>();

            _accountInformation = new AccountInformation
            {
                OwnerEmail = "test@test.com",
                OrganisationName = "Test Organisation",
                DasAccountName = "My Account",
                OrganisationId = 123,
                OrganisationStatus = "active",
                OrganisationRegisteredAddress = "My Address",
                OrganisationSource = "companies house",
                OrgansiationCreatedDate = new DateTime(2000,01,10),
                DateRegistered = new DateTime(2016,10,30),
                OrganisationNumber = "123ADFC",
                DasAccountId = "45TFD",
                PayeSchemeName = "Scheme name 1"
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAccountsByHashedIdQuery>()))
                .ReturnsAsync(new GetEmployerAccountsByHashedIdResponse { Accounts = new List<AccountInformation> { _accountInformation } });

            _controller = new EmployerAccountsInformationController(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledWithTheGivenParameters()
        {
            //Arrange
            var hashedId = "ABC123";

            //Act
            await _controller.Index(hashedId);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetEmployerAccountsByHashedIdQuery>(c => c.HashedAccountId == hashedId)));
        }

        [Test]
        public async Task ThenIfTheHashedIdIsNotProvidedABadRequestIsReturned()
        {
            //Act
            var actual = await _controller.Index("");

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetEmployerAccountsByHashedIdQuery>()),Times.Never);
            _logger.Verify(x=>x.Info("API AccountsInformation - hashed account id not provided"));
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<BadRequestResult>(actual);
        }

        [Test]
        public async Task ThenTheDataIsReturnedInTheResponse()
        {
            //Arrange
            var hashedAccountId = "ABC123";

            //Act
            var actual = await _controller.Index(hashedAccountId);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<OkNegotiatedContentResult<List<AccountInformationViewModel>>>(actual);
            var model = actual as OkNegotiatedContentResult<List<AccountInformationViewModel>>;
            Assert.IsNotNull(model?.Content);
            Assert.AreEqual(1,model.Content.Count);
            var item = model.Content.First();
            Assert.AreEqual(_accountInformation.DasAccountName,item.DasAccountName);
            Assert.AreEqual(_accountInformation.DateRegistered,item.DateRegistered);
            Assert.AreEqual(_accountInformation.OrganisationId, item.OrganisationId);
            Assert.AreEqual(_accountInformation.OrganisationRegisteredAddress,item.OrganisationRegisteredAddress);
            Assert.AreEqual(_accountInformation.OrganisationSource,item.OrganisationSource);
            Assert.AreEqual(_accountInformation.OrganisationStatus,item.OrganisationStatus);
            Assert.AreEqual(_accountInformation.OrganisationName, item.OrganisationName);
            Assert.AreEqual(_accountInformation.OwnerEmail,item.OwnerEmail);
            Assert.AreEqual(_accountInformation.OrgansiationCreatedDate,item.OrgansiationCreatedDate);
            Assert.AreEqual(_accountInformation.DasAccountId,item.DasAccountId);
            Assert.AreEqual(_accountInformation.OrganisationNumber, item.OrganisationNumber);
            Assert.AreEqual(_accountInformation.PayeSchemeName, item.PayeSchemeName);
        }

        [Test]
        public async Task ThenABadRequestIsReturnedWhenTheMediatorCallIsntValid()
        {
            //Arrange
            var hashedId = "ABC123";
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAccountsByHashedIdQuery>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> {{"", ""}}));

            //Act
            var actual = await _controller.Index(hashedId);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<BadRequestResult>(actual);
        }
    }
}
