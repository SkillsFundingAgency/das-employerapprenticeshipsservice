using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.AccountHelper;
using SFA.DAS.EAS.Portal.Application.Commands.Paye;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.Commands.Paye
{
    [Parallelizable]
    [TestFixture]
    class WhenPayeSchemeAdded
    {
        PayeSchemeAddedCommandHandler _sut;
        Mock<IAccountHelperService> _accountHelperServiceMock;
        Mock<IAccountDocumentService> _accountServiceMock;
        AccountDocument _accountDoc;
        IFixture Fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _accountServiceMock = new Mock<IAccountDocumentService>();
            _accountHelperServiceMock = new Mock<IAccountHelperService>();
            _sut = new PayeSchemeAddedCommandHandler(_accountHelperServiceMock.Object, _accountServiceMock.Object);

            Fixture.Customize<Account>(a => a
                .Without(a2 => a2.PayeSchemes)
                .With(acc => acc.Id, 1));
            _accountDoc = Fixture
                .Build<AccountDocument>()
                
                .Create();

            _accountHelperServiceMock.Setup(mock => mock.GetOrCreateAccount(1, It.IsAny<CancellationToken>())).ReturnsAsync(_accountDoc);
        }

        [Test]
        [Category("UnitTest")]
        public async Task ShouldGetAccount()
        {
            // Arrange
            var userRef = Guid.NewGuid();
            var payeCommand = new PayeSchemeAddedCommand(1, "Bob", userRef, "payepayepaye", new DateTime(2019, 5, 31));
            

            // Act
            await _sut.Handle(payeCommand);

            // Assert
            _accountHelperServiceMock.VerifyAll();
        }

        [Test]
        [TestCase(0, Description = "No existing PAYE schemes")]
        [TestCase(2, Description = "Multiple existing PAYE, not same")]
        [Category("UnitTest")]
        public async Task ShouldAddPayeSchemeToAccountWhenNotExists(int numOfExistingPayeSchemes)
        {
            // Arrange
            _accountDoc.Account.PayeSchemes = Fixture.CreateMany<PayeScheme>(numOfExistingPayeSchemes).ToList();
            var userRef = Guid.NewGuid();
            var payeCommand = new PayeSchemeAddedCommand(1, "Bob", userRef, "payepayepaye", new DateTime(2019, 5, 31));

            AccountDocument resultAccountDoc = null;
            _accountServiceMock.Setup(mock => mock.Save(It.Is<AccountDocument>(ad => ad.AccountId == 1), It.IsAny<CancellationToken>()))
                .Callback((AccountDocument accountDoc, CancellationToken ct) =>
                {
                    resultAccountDoc = accountDoc;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _sut.Handle(payeCommand);

            // Assert
            resultAccountDoc.Account.PayeSchemes.Count.Should().Be(numOfExistingPayeSchemes + 1);
        }

        [Test]
        [Category("UnitTest")]
        public async Task ShouldNotAddPayeSchemeToAccountIfAlreadyExists()
        {
            // Arrange
            var existingPaye = new PayeScheme { PayeRef = "payepayepaye" };
            _accountDoc.Account.PayeSchemes = Fixture.CreateMany<PayeScheme>(3).ToList();
            _accountDoc.Account.PayeSchemes.Add(existingPaye);
            var userRef = Guid.NewGuid();
            var payeCommand = new PayeSchemeAddedCommand(1, "Bob", userRef, "payepayepaye", new DateTime(2019, 5, 31));

            AccountDocument resultAccountDoc = null;
            _accountServiceMock.Setup(mock => mock.Save(It.Is<AccountDocument>(ad => ad.AccountId == 1), It.IsAny<CancellationToken>()))
                .Callback((AccountDocument accountDoc, CancellationToken ct) =>
                {
                    resultAccountDoc = accountDoc;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _sut.Handle(payeCommand);

            // Assert
            resultAccountDoc.Account.PayeSchemes.Count.Should().Be(4);
        }
    }
}
