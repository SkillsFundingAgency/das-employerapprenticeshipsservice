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
using SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccountsByDateRange;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsInformationTests
{
    public class WhenIGetAccountInformation
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
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetPagedEmployerAccountsByDateRangeQuery>()))
                .ReturnsAsync(new GetPagedEmployerAccountsByDateRangeResponse {Accounts = new Accounts<AccountInformation> { AccountList = new List<AccountInformation> { _accountInformation },AccountsCount = 1} });

            _controller = new EmployerAccountsInformationController(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledWithTheGivenParameters()
        {
            //Arrange
            var fromDate = "2016-09-21";
            var toDate = "2016-10-30";
            var pageNumber = 10;
            var pageSize = 100;

            //Act
            await _controller.Index(fromDate,toDate,pageSize,pageNumber);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetPagedEmployerAccountsByDateRangeQuery>(
                                    c=>c.PageNumber.Equals(pageNumber)
                                    && c.PageSize.Equals(pageSize)
                                    && c.FromDate.Equals(Convert.ToDateTime(fromDate))
                                    && c.ToDate.Equals(Convert.ToDateTime(toDate).AddDays(1).AddSeconds(-1))
                                    )));
        }

        [Test]
        public async Task ThenIfTheDatesAreInvalidABadRequestIsReturned()
        {
            //Act
            var actual = await _controller.Index("150", "20", 0, 10);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetPagedEmployerAccountsByDateRangeQuery>()),Times.Never);
            _logger.Verify(x=>x.Info("API AccountsInformation - Invalid dates entered fromDate:150 toDate:20"));
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<BadRequestResult>(actual);
        }

        [Test]
        public async Task ThenTheDataIsReturnedInTheResponse()
        {
            //Arrange
            var fromDate = "2016-01-01";
            var toDate = "2016-10-30";
            var pageNumber = 10;
            var pageSize = 100;

            //Act
            var actual = await _controller.Index(fromDate, toDate, pageSize, pageNumber);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PagedApiResponseViewModel<AccountInformationViewModel>>>(actual);
            var model = actual as OkNegotiatedContentResult<PagedApiResponseViewModel<AccountInformationViewModel>>;
            Assert.IsNotNull(model?.Content?.Data);
            Assert.AreEqual(1,model.Content.Data.Count);
            var item = model.Content.Data.First();
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
            var fromDate = "2016-01-01";
            var toDate = "2016-10-30";
            var pageNumber = 10;
            var pageSize = 100;
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetPagedEmployerAccountsByDateRangeQuery>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> {{"", ""}}));

            //Act
            var actual = await _controller.Index(fromDate, toDate, pageSize, pageNumber);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<BadRequestResult>(actual);
        }
    }
}
