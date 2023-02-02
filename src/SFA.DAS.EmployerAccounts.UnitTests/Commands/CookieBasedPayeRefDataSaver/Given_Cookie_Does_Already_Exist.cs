using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.PayeRefData;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CookieBasedPayeRefDataSaver;

[TestFixture]
public sealed class Given_Cookie_Does_Already_Exist
{
    private EmployerAccounts.Commands.PayeRefData.CookieBasedPayeRefDataSaver _sut;
    private EmployerAccountOrganisationData _organisationData;

    public Given_Cookie_Does_Already_Exist()
    {
        setupOrganisationData();

        CookieRepository = new Mock<ICookieStorageService<EmployerAccountData>>();

        CookieRepository
            .Setup(
                m =>
                    m.Get(It.IsAny<string>()))
            .Returns(
                new EmployerAccountData
                {
                    EmployerAccountOrganisationData = _organisationData,
                    EmployerAccountPayeRefData = new EmployerAccountPayeRefData()
                });

        _sut = new EmployerAccounts.Commands.PayeRefData.CookieBasedPayeRefDataSaver(CookieRepository.Object);
    }

    [Test]
    public Task Then_CreateCookie_Is_Not_Called()
    {
        Handle();

        CookieRepository
            .Verify(
                m => m.Create(
                    It.IsAny<EmployerAccountData>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()),
                Times.Never
            );

        return Task.CompletedTask;
    }

    [Test]
    public Task Then_UpdateCookie_Is_Called()
    {
        Handle();

        CookieRepository
            .Verify(
                m =>
                    m.Update(
                        It.IsAny<string>(),
                        It.IsAny<EmployerAccountData>()));

        return Task.CompletedTask;
    }

    [Test]
    public Task Then_Existing_PayeRef_Data_Is_Unchanged()
    {
        Handle();

        CookieRepository
            .Verify(
                m =>
                    m.Update(
                        It.IsAny<string>(),
                        It.Is<EmployerAccountData>(data => payeRefDataIsUnchanged(data.EmployerAccountOrganisationData))));

        return Task.CompletedTask;
    }

    private bool payeRefDataIsUnchanged(EmployerAccountOrganisationData data)
    {
        data.OrganisationName.Should().Be(_organisationData.OrganisationName);
        data.OrganisationReferenceNumber.Should().Be(_organisationData.OrganisationReferenceNumber);
        data.OrganisationRegisteredAddress.Should().Be(_organisationData.OrganisationRegisteredAddress);

        return true;
    }

    public Mock<ICookieStorageService<EmployerAccountData>> CookieRepository { get; set; }

    private Task Handle()
    {
        return
            _sut.Handle(new SavePayeRefData(new EmployerAccountPayeRefData()), CancellationToken.None);
    }

    private void setupOrganisationData()
    {
        _organisationData = new EmployerAccountOrganisationData
        {

            OrganisationName = @"{FBC24A46-AC40-4477-8D7E-9A514C0F4AAE}",
            OrganisationReferenceNumber = @"{64439F80-CE0A-4E7E-924A-57B4420EF4FF}",
            OrganisationRegisteredAddress = @"{74988389-DBED-4996-9576-7F61426585CB}",
        };
    }
}