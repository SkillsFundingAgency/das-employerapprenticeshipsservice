using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.UserProfile;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class UserAornPayeLockService : IUserAornPayeLockService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserAornPayeLockConfiguration _configuration;

        public UserAornPayeLockService(IUserRepository userRepository, EmployerAccountsConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration.UserAornPayeLock;
        }

        public async Task UpdateUserAornPayeAttempt(Guid userRef, bool success)
        {
            await _userRepository.UpdateAornPayeQueryAttempt(userRef, success);
        }

        public async Task<UserAornPayeStatus> UserAornPayeStatus(Guid userRef)
        {
            var attempts = (await _userRepository.GetAornPayeQueryAttempts(userRef)).OrderByDescending(a => a.Date).ToList();

            if (attempts.Count < _configuration.NumberOfPermittedAttempts ||
                attempts.First() < DateTime.Now.AddMinutes(-_configuration.PermittedAttemptsTimeSpanMinutes) ||
                attempts.Count(a => (attempts.First() - a).TotalMinutes <= _configuration.PermittedAttemptsTimeSpanMinutes) < _configuration.NumberOfPermittedAttempts)
            {
                return new UserAornPayeStatus
                {
                    IsLocked = false,
                    RemainingAttempts = _configuration.NumberOfPermittedAttempts - 
                                        attempts.Count(a => (DateTime.Now - a).TotalMinutes <= _configuration.PermittedAttemptsTimeSpanMinutes)
                };
            }

            return new UserAornPayeStatus
            {
                IsLocked = true,
                BeginTime = attempts.First(),
                EndTime = attempts.First().AddMinutes(_configuration.LockoutTimeSpanMinutes),
                RemainingAttempts = 0
            };
        }
    }
}
