using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Responses.UserAccounts;
using SFA.DAS.EmployerAccounts.Models.UserAccounts;

namespace SFA.DAS.EmployerAccounts.UnitTests.Infrastructure.OuterApi.Response;

public class WhenCastingFromApiResponseToEmployerUserAccounts
{
    [Test, AutoData]
    public void Then_The_Values_Are_Mapped(GetUserAccountsResponse source)
    {
        //Act
        var actual = (EmployerUserAccounts) source;

        //Assert
        actual.Should().BeEquivalentTo(source, options=>options.Excluding(x=>x.UserAccounts));
        actual.EmployerAccounts.Should().BeEquivalentTo(source.UserAccounts);
    }

    [Test, AutoData]
    public void Then_If_No_Accounts_Then_Empty_List_Returned(GetUserAccountsResponse source)
    {
        //Arrange
        source.UserAccounts = null;
        
        //Act
        var actual = (EmployerUserAccounts) source;

        //Assert
        actual.Should().BeEquivalentTo(source, options=>options.Excluding(x=>x.UserAccounts));
        actual.EmployerAccounts.Should().BeEmpty();
    }

    [Test, AutoData]
    public void Then_If_No_Values_Null_Returned()
    {   
        //Act
        var actual = (EmployerUserAccounts) ((GetUserAccountsResponse)null);

        //Assert
        actual.FirstName.Should().BeNullOrEmpty();
        actual.LastName.Should().BeNullOrEmpty();
        actual.Email.Should().BeNullOrEmpty();
        actual.EmployerUserId.Should().BeNullOrEmpty();
        actual.EmployerAccounts.Should().BeEmpty();
        actual.IsSuspended.Should().BeFalse();
    }
}