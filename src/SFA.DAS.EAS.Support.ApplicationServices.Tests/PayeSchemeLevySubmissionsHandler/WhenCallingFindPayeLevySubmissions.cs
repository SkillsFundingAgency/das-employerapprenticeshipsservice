using HMRC.ESFA.Levy.Api.Types;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.PayeSchemeLevySubmissionsHandler
{
    [TestFixture]
    public class WhenCallingFindPayeLevySubmissions
    {

        private Mock<IAccountRepository>? _accountRepository;
        private Mock<ILevySubmissionsRepository>? _levySubmissionsRepository;
        private Mock<IPayeSchemeObfuscator>? _payeSchemeObfuscator;
        private Mock<ILogger<PayeLevySubmissionsHandler>>? _log;
        private Mock<IPayRefHashingService>? _hashingService;

        private const string AccountId = "45789456123";
        private const string HashedPayeRef = "HASHPAYEID";
        private const string ActualPayeRef = "155/5555";

        [SetUp]
        public void SetUp()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _levySubmissionsRepository = new Mock<ILevySubmissionsRepository>();
            _payeSchemeObfuscator = new Mock<IPayeSchemeObfuscator>();
            _log = new Mock<ILogger<PayeLevySubmissionsHandler>>();
            _hashingService = new Mock<IPayRefHashingService>();

        }

        [Test]
        public async Task ShouldReturnLevyResponseWithUnexpectedErrorStatusCodeWhenExceptionOccursWithHmrcApiClient()
        {
            // Arrange
            IPayeLevySubmissionsHandler payeLevySubmissionsHandler =
                new PayeLevySubmissionsHandler(
                    _accountRepository!.Object,
                    _levySubmissionsRepository!.Object,
                    _payeSchemeObfuscator!.Object,
                    _log!.Object,
                    _hashingService!.Object);
            PayeLevySubmissionsResponse payeLevySubmissionsResponse;
            PayeLevySubmissionsResponseCodes? expectedStatusCode = PayeLevySubmissionsResponseCodes.UnexpectedError;
            PayeLevySubmissionsResponseCodes? actualStatusCode;

            _hashingService
                .Setup(x => x.DecodeValueToString(HashedPayeRef))
                .Returns(ActualPayeRef);
      
            _accountRepository
                .Setup(x => x.Get(
                    It.Is<string>(y => y == AccountId),
                    It.Is<AccountFieldsSelection>(y => y == AccountFieldsSelection.PayeSchemes)))
                .Returns(Task.FromResult(GenerateTestAccount()));
            
            _levySubmissionsRepository
                .Setup(x => x.Get(It.Is<string>(y => y == ActualPayeRef)))
                .Callback(() => throw new Exception("Some HRMC Exception!"));

            // Act
            payeLevySubmissionsResponse = await payeLevySubmissionsHandler.FindPayeSchemeLevySubmissions(
                AccountId,
                HashedPayeRef);

            actualStatusCode = payeLevySubmissionsResponse.StatusCode;

            // Assert
            Assert.That(actualStatusCode, Is.EqualTo(expectedStatusCode));
        }

        [Test]
        public async Task ShouldReturnLevyResponseAndCallRequiredMethods()
        {
            var levyDeclarations = new LevyDeclarations
            {
                Declarations = new List<Declaration>
                    {
                        new()
                        {
                            Id = "002",
                             SubmissionTime = new DateTime(2017, 4, 1)
                        },
                        new()
                        {
                             Id = "003",
                           SubmissionTime = new DateTime(2017, 4, 1)
                        }
                    }
            };

            var accountModel = GenerateTestAccount();

            _accountRepository!
                .Setup(x => x.Get(AccountId, AccountFieldsSelection.PayeSchemes))
                .Returns(Task.FromResult(accountModel));

            _hashingService!
                .Setup(x => x.DecodeValueToString(HashedPayeRef))
                .Returns(ActualPayeRef);

            _payeSchemeObfuscator!
                .Setup(x => x.ObscurePayeScheme(ActualPayeRef))
                 .Returns("1*******5");

            _levySubmissionsRepository!
                .Setup(x => x.Get(ActualPayeRef))
                .Returns(Task.FromResult(levyDeclarations));

            var sut = new PayeLevySubmissionsHandler(_accountRepository.Object,
                                                    _levySubmissionsRepository.Object,
                                                    _payeSchemeObfuscator.Object,
                                                    _log!.Object,
                                                    _hashingService.Object);


            var response = await sut.FindPayeSchemeLevySubmissions(AccountId, HashedPayeRef);

            _payeSchemeObfuscator
              .Verify(x => x.ObscurePayeScheme(ActualPayeRef), Times.Once);

            _hashingService
                .Verify(x => x.DecodeValueToString(HashedPayeRef), Times.Once);

            _levySubmissionsRepository
                .Verify(x => x.Get(ActualPayeRef), Times.Once);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.LevySubmissions, Is.Not.Null);
            Assert.That(response.LevySubmissions.Declarations, Has.Count.EqualTo(2));
        }

        private static Core.Models.Account GenerateTestAccount()
        {
            var toReturn = new Core.Models.Account
            {
                HashedAccountId = AccountId,
                DasAccountName = "TEST",
                PayeSchemes = new List<PayeSchemeModel>
                {
                    new()
                    {
                        Ref = ActualPayeRef
                    }
               }
            };

            return toReturn;
        }

    }
}
