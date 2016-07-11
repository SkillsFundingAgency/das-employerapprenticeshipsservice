using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUsers;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Tests.Queries.GetUsersTests
{
    public class WhenIGetUsers
    {
        private Mock<IUserRepository> _userRepository;
        private GetUsersQueryHandler _getUsersQueryHandler;
        private List<User> _users;
        private User _user;

        [SetUp]
        public void Arrange()
        {
            _userRepository = new Mock<IUserRepository>();
            _user = new User { Email = "test@local.com", FirstName = "Test", LastName = "Tester", UserId = Guid.NewGuid().ToString() };
            _users = new List<User> { _user };
            _userRepository.Setup(x => x.GetAllUsers()).ReturnsAsync(new Users {UserList = _users});

            _getUsersQueryHandler = new GetUsersQueryHandler(_userRepository.Object);
        }

        [Test]
        public async Task ThenTheUserRepositoryIsCalledToGetAllUsers()
        {
            //Act
            await _getUsersQueryHandler.Handle(new GetUsersQuery());

            //Assert
            _userRepository.Verify(x => x.GetAllUsers(), Times.Once);
        }

        [Test]
        public async Task ThenAListOfUsersIsReturned()
        {
            //Act
            var actual = await _getUsersQueryHandler.Handle(new GetUsersQuery());

            //Assert    
            Assert.IsAssignableFrom<GetUsersQueryResponse>(actual);
        }

        [Test]
        public async Task ThenTheListReturnedConatinsTheUsersFromTheRepository()
        {
            //Act
            var actual = await _getUsersQueryHandler.Handle(new GetUsersQuery());

            //Assert
            Assert.IsNotEmpty(actual.Users);
            Assert.AreEqual(1, actual.Users.Count);
            Assert.Contains(_user,actual.Users);
        }
    }
}
