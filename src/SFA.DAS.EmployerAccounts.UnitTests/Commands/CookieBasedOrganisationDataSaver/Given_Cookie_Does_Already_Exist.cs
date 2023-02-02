using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.OrganisationData;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CookieBasedOrganisationDataSaver;

[TestFixture]
public sealed class Given_Cookie_Does_Already_Exist
{
    private EmployerAccounts.Commands.OrganisationData.CookieBasedOrganisationDataSaver _sut;
    private EmployerAccountPayeRefData _payeRefData;

    public Given_Cookie_Does_Already_Exist()
    {
        setupPayeRefData();

        CookieRepository = new Mock<ICookieStorageService<EmployerAccountData>>();

        CookieRepository
            .Setup(
                m =>
                    m.Get(It.IsAny<string>()))
            .Returns(
                new EmployerAccountData
                {
                    EmployerAccountOrganisationData = new EmployerAccountOrganisationData(),
                    EmployerAccountPayeRefData = _payeRefData
                });

        _sut = new EmployerAccounts.Commands.OrganisationData.CookieBasedOrganisationDataSaver(CookieRepository.Object);
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
                        It.Is<EmployerAccountData>(data => payeRefDataIsUnchanged(data.EmployerAccountPayeRefData))));

        return Task.CompletedTask;
    }

    private bool payeRefDataIsUnchanged(EmployerAccountPayeRefData data)
    {
        data.AccessToken.Should().Be(_payeRefData.AccessToken);
        data.EmpRefNotFound.Should().Be(_payeRefData.EmpRefNotFound);
        data.EmployerRefName.Should().Be(_payeRefData.EmployerRefName);
        data.PayeReference.Should().Be(_payeRefData.PayeReference);
        data.RefreshToken.Should().Be(_payeRefData.RefreshToken);

        return true;
    }

    public Mock<ICookieStorageService<EmployerAccountData>> CookieRepository { get; set; }

    private Task Handle()
    {
        return
            _sut
                .Handle(
                    new SaveOrganisationData(new EmployerAccountOrganisationData()), CancellationToken.None);
    }

    private void setupPayeRefData()
    {
        _payeRefData = new EmployerAccountPayeRefData
        {
            AccessToken = @"{FBC24A46-AC40-4477-8D7E-9A514C0F4AAE}",
            EmployerRefName = @"{64439F80-CE0A-4E7E-924A-57B4420EF4FF}",
            EmpRefNotFound = false,
            PayeReference = @"{74988389-DBED-4996-9576-7F61426585CB}",
            RefreshToken = @"{0994787F-6890-41B5-9F67-FCF6D036FF7A}"
        };
    }
}