using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class UserAornPayeLockService : IUserAornPayeLockService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserAornPayeLockConfiguration _configuration;
        private readonly ILog _logger;

        public UserAornPayeLockService(
            IUserRepository userRepository, 
            EmployerAccountsConfiguration configuration,
            ILog logger)
        {
            _logger = logger;
            _userRepository = userRepository;
            _configuration = configuration.UserAornPayeLock;
        }

        public async Task<bool> UpdateUserAornPayeAttempt(Guid userRef, bool success)
        {
            try
            {
                await _userRepository.UpdateAornPayeQueryAttempt(userRef, success);
            }
            catch (Exception e)
            {
                _logger.Error(e, "An error occurred when calling the Aorn Lock Service.");
                return false;
            }
            
            return true;
        }

        public async Task<UserAornPayeStatus> UserAornPayeStatus(Guid userRef)
        {
            try
            {
                var lockLimit = DateTime.Now.AddMinutes(-_configuration.LockoutTimeSpanMinutes);
                var attempts = (await _userRepository.GetAornPayeQueryAttempts(userRef)).Where(a => a > lockLimit).OrderByDescending(a => a).ToList();

                if (attempts.Count >= _configuration.NumberOfPermittedAttempts)
                {
                    for (var i = 0; i <= attempts.Count - _configuration.NumberOfPermittedAttempts; ++i)
                    {
                        if ((attempts[i] - attempts[i + _configuration.NumberOfPermittedAttempts - 1]).TotalMinutes <= _configuration.PermittedAttemptsTimeSpanMinutes)
                        {
                            return new UserAornPayeStatus
                            {
                                IsLocked = true,
                                RemainingLock = Convert.ToInt32((attempts.First().AddMinutes(_configuration.LockoutTimeSpanMinutes) - DateTime.Now).TotalMinutes),
                                RemainingAttempts = 0,
                                AllowedAttempts = _configuration.NumberOfPermittedAttempts
                            };
                        }
                    }
                }

                return new UserAornPayeStatus
                {
                    IsLocked = false,
                    RemainingAttempts = _configuration.NumberOfPermittedAttempts -
                                        attempts.Count(a => (DateTime.Now - a).TotalMinutes <= _configuration.PermittedAttemptsTimeSpanMinutes),
                    AllowedAttempts = _configuration.NumberOfPermittedAttempts
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "An error occurred when calling the Aorn Lock Service.");

                return new UserAornPayeStatus
                {
                    IsLocked = true,
                    RemainingAttempts = 0,
                    AllowedAttempts = _configuration.NumberOfPermittedAttempts
                };
            }
        }
    }
}
