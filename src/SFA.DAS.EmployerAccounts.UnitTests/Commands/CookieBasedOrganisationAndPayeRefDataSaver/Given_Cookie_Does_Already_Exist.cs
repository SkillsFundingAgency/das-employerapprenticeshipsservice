using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.OrganisationAndPayeRefData;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CookieBasedOrganisationAndPayeRefDataSaver;

[TestFixture]
public sealed class Given_Cookie_Does_Already_Exist 
{
    private CookieBasedOrganisationAndPayeDataSaver _sut;
    private EmployerAccountOrganisationData _organisationData;

    public Given_Cookie_Does_Already_Exist()
    {
        SetupOrganisationData();

        CookieRepository = new Mock<ICookieStorageService<EmployerAccountData>>();

        CookieRepository
            .Setup(m => m.Get(It.IsAny<string>()))
            .Returns(
                new EmployerAccountData
                {
                    EmployerAccountOrganisationData = _organisationData,
                    EmployerAccountPayeRefData = new EmployerAccountPayeRefData()
                });

        _sut = new CookieBasedOrganisationAndPayeDataSaver(CookieRepository.Object);
    }

    [Test]
    public Task Then_CreateCookie_Is_Not_Called()
    {
        Handle();
        CookieRepository.Verify(m => m.Create(It.IsAny<EmployerAccountData>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);

        return Task.CompletedTask;
    }

    [Test]
    public Task Then_UpdateCookie_Is_Called()
    {
        Handle();
        CookieRepository.Verify(m => m.Update(It.IsAny<string>(), It.IsAny<EmployerAccountData>()));

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

    private void SetupOrganisationData()
    {
        _organisationData = new EmployerAccountOrganisationData
        {
            OrganisationName = @"{FBC24A46-AC40-4477-8D7E-9A514C0F4AAE}",
            OrganisationReferenceNumber = @"{64439F80-CE0A-4E7E-924A-57B4420EF4FF}",
            OrganisationRegisteredAddress = @"{74988389-DBED-4996-9576-7F61426585CB}",
        };
    }
}