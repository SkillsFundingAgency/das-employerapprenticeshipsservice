using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.OrganisationAndPayeRefData;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CookieBasedOrganisationAndPayeRefDataSaver;

[TestFixture]
public sealed class Given_Cookie_Does_Not_Already_Exist
{
    private CookieBasedOrganisationAndPayeDataSaver _sut;

    public Given_Cookie_Does_Not_Already_Exist()
    {
        CookieRepository = new Mock<ICookieStorageService<EmployerAccountData>>();

        CookieRepository
            .Setup(
                m =>
                    m.Get(It.IsAny<string>()))
            .Returns((EmployerAccountData)null);

        _sut = new CookieBasedOrganisationAndPayeDataSaver(CookieRepository.Object);
    }

    [Test]
    public Task Then_CreateCookie_Is_Called()
    {
        Handle();
        CookieRepository
            .Verify(
                m => m.Create(
                    It.IsAny<EmployerAccountData>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()));

        return Task.CompletedTask;
    }

    [Test]
    public Task Then_UpdateCookie_Is_Not_Called()
    {
        Handle();

        CookieRepository
            .Verify(
                m =>
                    m.Update(
                        It.IsAny<string>(),
                        It.IsAny<EmployerAccountData>()),
                Times.Never);

        return Task.CompletedTask;
    }

    public Mock<ICookieStorageService<EmployerAccountData>> CookieRepository { get; set; }

    private Task Handle()
    {
        return
            _sut
                .Handle(
                    new SaveOrganisationAndPayeData(new EmployerAccountOrganisationData(), new EmployerAccountPayeRefData()), CancellationToken.None);
    }
}