using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Components.DictionaryAdapter;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.Payments.Events.Api.Client;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.PaymentServiceTests
{
    internal class WhenIGetAccountPayments
    {
        private const string PeriodEnd = "R12-13";
        private const long AccountId = 2;
        private const string StandardCourseName = "Standard Course";
        private const string FrameworkCourseName = "Framework Course";

        private Mock<IApprenticeshipInfoServiceWrapper> _apprenticeshipInfoService;
        private Mock<IEmployerCommitmentApi> _commitmentsApiClient;
        private Mock<IPaymentsEventsApiClient> _paymentsApiClient;
        private Mock<IMapper> _mapper;
        private Mock<ILogger> _logger;

        private PaymentService _paymentService;
        private Framework _framework;
        private Standard _standard;
        private Apprenticeship _apprenticeship;
        private Provider _provider;
        private PaymentDetails _standardPayment;
        private PaymentDetails _frameworkPayment;

        [SetUp]
        public void Arrange()
        {
            SetupTestModels();
            SetupPaymentsApiMock();
            SetupCommitmentsApiMock();
            SetupApprenticeshipServiceMock();
            SetupMapperMock();
            SetupLoggerMock();

            _paymentService = new PaymentService(
                _paymentsApiClient.Object,
                _commitmentsApiClient.Object,
                _apprenticeshipInfoService.Object,
                _mapper.Object,
                _logger.Object);
        }

        private void SetupLoggerMock()
        {
            _logger = new Mock<ILogger>();
            _logger.Setup(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()));
            _logger.Setup(x => x.Warn(It.IsAny<Exception>(), It.IsAny<string>()));
        }

        private void SetupMapperMock()
        {
            _mapper = new Mock<IMapper>();
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Payment>()))
                .Returns(() => _standardPayment);
        }

        [Test]
        public async Task ThenThePaymentsApiIsCalledToGetPaymentData()
        {
            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            _paymentsApiClient.Verify(x => x.GetPayments(PeriodEnd, AccountId.ToString(), 1));
        }

        [Test]
        public async Task ThenTheCommitmentsApiIsCalledToGetApprenticeDetails()
        {
            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            _commitmentsApiClient.Verify(x => x.GetEmployerApprenticeship(AccountId, _apprenticeship.Id), Times.Once);
        }

        [Test]
        public async Task ThenTheAppreticeshipsApiIsCalledToGetProviderDetails()
        {

            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            _apprenticeshipInfoService.Verify(x => x.GetProvider(_provider.Ukprn), Times.Once);
        }

        [Test]
        public async Task ThenTheAppreticeshipsApiIsCalledToGetStandardDetails()
        {
            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            _apprenticeshipInfoService.Verify(x => x.GetStandardsAsync(false), Times.Once);
            _apprenticeshipInfoService.Verify(x => x.GetFrameworksAsync(false), Times.Never);
        }

        [Test]
        public async Task ThenTheAppreticeshipsApiIsCalledToGetFrameworkDetails()
        {
            //Arrange
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Payment>()))
                     .Returns(() => _frameworkPayment);

            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            _apprenticeshipInfoService.Verify(x => x.GetFrameworksAsync(false), Times.Once);
            _apprenticeshipInfoService.Verify(x => x.GetStandardsAsync(false), Times.Never);
        }

        [Test]
        public async Task ThenIShouldGetPaymentDetails()
        {
            //Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            Assert.AreEqual(AccountId, details.First().EmployerAccountId);
        }

        [Test]
        public async Task ThenIShouldGetCorrectPeriodEnd()
        {
            //Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            Assert.AreEqual(details.First().PeriodEnd, PeriodEnd);
        }

        [Test]
        public async Task ThenIShouldGetCorrectProviderName()
        {
            //Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            Assert.AreEqual(_provider.ProviderName, details.First().ProviderName);
        }

        [Test]
        public async Task ThenIShouldGetCorrectStandardCourseName()
        {
            //Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            Assert.AreEqual(StandardCourseName, details.First().CourseName);
        }

        [Test]
        public async Task ThenIShouldGetCorrectFrameworkCourseName()
        {
            //Arrange
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Payment>()))
                    .Returns(() => _frameworkPayment);

            //Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            Assert.AreEqual(FrameworkCourseName, details.First().CourseName);
        }

        [Test]
        public async Task ThenIShouldGetCorrectApprenticeDetails()
        {
            //Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            var apprenticeName = $"{_apprenticeship.FirstName} {_apprenticeship.LastName}"; 

            Assert.AreEqual(apprenticeName, details.First().ApprenticeName);
            Assert.AreEqual(_apprenticeship.NINumber, details.First().ApprenticeNINumber);
        }

        [Test]
        public async Task ThenShouldLogWarningIfCommitmentsApiCallFails()
        {
            //Arrange
            _commitmentsApiClient.Setup(x => x.GetEmployerApprenticeship(It.IsAny<long>(), It.IsAny<long>()))
                .Throws<WebException>();

            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            _logger.Verify(x => x.Warn(It.IsAny<Exception>(), 
                $"Unable to get Apprenticeship with provider ID {_provider.Ukprn} and " + 
                $"apprenticeship ID {_standardPayment.ApprenticeshipId} from commitments API."), Times.Once);
        }

        [Test]
        public async Task ThenShouldLogErrorIfPaymentsApiCallFails()
        {
            //Arrange
            _paymentsApiClient.Setup(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                              .Throws<WebException>();

            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            _logger.Verify(x => x.Error(It.IsAny<Exception>(), 
                $"Unable to get payment information for {PeriodEnd} accountid {AccountId}"), Times.Once);
        }

        [Test]
        public async Task ThenShouldLogWarningIfApprenticeshipsApiCallFailsWhenGettingProviderDetails()
        {
            //Arrange
            _apprenticeshipInfoService.Setup(x => x.GetProvider(It.IsAny<int>()))
                                      .Throws<WebException>();

            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            _logger.Verify(x => x.Warn(It.IsAny<Exception>(), 
                $"Unable to get provider details with UKPRN {_provider.Ukprn} from apprenticeship API."), Times.Once);
        }

        [Test]
        public async Task ThenShouldLogWarningIfApprenticeshipsApiCallFailsWhenGettingStandards()
        {
            //Arrange
            _apprenticeshipInfoService.Setup(x => x.GetStandardsAsync(It.IsAny<bool>()))
                                      .Throws<WebException>();

            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            _logger.Verify(x => x.Warn(It.IsAny<Exception>(), "Could not get standards from apprenticeship API."), Times.Once);
        }

        [Test]
        public async Task ThenShouldLogWarningIfApprenticeshipsApiCallFailsWhenGettingFrameworks()
        {
            //Arrange
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Payment>()))
                  .Returns(() => _frameworkPayment);
            _apprenticeshipInfoService.Setup(x => x.GetFrameworksAsync(It.IsAny<bool>()))
                                      .Throws<WebException>();

            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId.ToString());

            //Assert
            _logger.Verify(x => x.Warn(It.IsAny<Exception>(), "Could not get frameworks from apprenticeship API."), Times.Once);
        }

        private void SetupTestModels()
        {
            _framework = new Framework
            {
                Title = FrameworkCourseName,
                FrameworkCode = 20,
                PathwayCode = 2,
                ProgrammeType = 3
            };

            _standard = new Standard
            {
                Id = "10",
                Code = 10,
                Title = StandardCourseName
            };

            _apprenticeship = new Apprenticeship
            {
                Id = 545646,
                FirstName = "John",
                LastName = "Doe",
                NINumber = "12345678"
            };

            _provider = new Provider
            {
                Id = 10,
                Ukprn = 74765,
                ProviderName = "Test Provider"
            };

            _standardPayment = new PaymentDetails
                {
                    Id = Guid.NewGuid().ToString(),
                    EmployerAccountId = AccountId,
                    Ukprn = _provider.Ukprn,
                    ApprenticeshipId = _apprenticeship.Id,
                    StandardCode = _standard.Code,
                   
                };

            _frameworkPayment = new PaymentDetails
                {
                    Id = Guid.NewGuid().ToString(),
                    EmployerAccountId = AccountId,
                    Ukprn = _provider.Ukprn,
                    ApprenticeshipId = _apprenticeship.Id,
                    FrameworkCode = _framework.FrameworkCode,
                    PathwayCode = _framework.PathwayCode,
                    ProgrammeType = _framework.ProgrammeType
                };
        }

        private void SetupPaymentsApiMock()
        {
            _paymentsApiClient = new Mock<IPaymentsEventsApiClient>();
            _paymentsApiClient.Setup(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new PageOfResults<Payment>
                {
                    Items = new[]{ new Payment() }
                });
        }

        private void SetupCommitmentsApiMock()
        {
            _commitmentsApiClient = new Mock<IEmployerCommitmentApi>();
            _commitmentsApiClient.Setup(x => x.GetEmployerApprenticeship(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_apprenticeship);
        }
        
        private void SetupApprenticeshipServiceMock()
        {
            _apprenticeshipInfoService = new Mock<IApprenticeshipInfoServiceWrapper>();
            _apprenticeshipInfoService.Setup(x => x.GetProvider(It.IsAny<int>()))
                .Returns(new ProvidersView
                {
                    Provider = _provider
                });

            _apprenticeshipInfoService.Setup(x => x.GetFrameworksAsync(false))
                .ReturnsAsync(new FrameworksView
                {
                    Frameworks = new EditableList<Framework>
                    {
                        _framework
                    }
                });

            _apprenticeshipInfoService.Setup(x => x.GetStandardsAsync(false))
                .ReturnsAsync(new StandardsView
                {
                    Standards = new EditableList<Standard>
                    {
                        _standard
                    }
                });
        }
    }
}
