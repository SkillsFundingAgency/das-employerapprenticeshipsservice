using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Infrastructure.Models;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.ChallengeRepository
{
    [TestFixture]
    public class WhenTestingChallengeRepository
    {
        [SetUp]
        public void Setup()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _unit = new Services.ChallengeRepository(_accountRepository.Object);
        }

        private Services.ChallengeRepository _unit;
        private Mock<IAccountRepository> _accountRepository;

        [Test]
        public async Task ItShouldReturnFalseWhenCheckDataHasIncorrectBalance()
        {
            var account = new Core.Models.Account
            {
                Transactions = new List<TransactionViewModel>
                {
                    new TransactionViewModel {Balance = 300m},
                    new TransactionViewModel {Balance = 700m}
                },
                PayeSchemes = new List<PayeSchemeModel>
                {
                    new PayeSchemeModel
                    {
                        AddedDate = DateTime.Today.AddMonths(-12),
                        Ref = "123/456789"
                    },
                    new PayeSchemeModel
                    {
                        AddedDate = DateTime.Today.AddMonths(-12),
                        Ref = "124/456789"
                    }
                }
            };
            var challengePermissionQuery = new ChallengePermissionQuery
            {
                Id = "123",
                Balance = "£1000",
                ChallengeElement1 = "1",
                ChallengeElement2 = "4",
                FirstCharacterPosition = 0,
                SecondCharacterPosition = 3
            };

            var balance = 999m;

            _accountRepository.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
                .ReturnsAsync(balance);

            var actual = await _unit.CheckData(account, challengePermissionQuery);

            Assert.IsFalse(actual);
        }

        [Test]
        public async Task ItShouldReturnFalseWhenCheckDataHasInvalidBalance()
        {
            var account = new Core.Models.Account
            {
                Transactions = new List<TransactionViewModel>
                {
                    new TransactionViewModel {Balance = 300m},
                    new TransactionViewModel {Balance = 700m}
                },
                PayeSchemes = new List<PayeSchemeModel>
                {
                    new PayeSchemeModel
                    {
                        AddedDate = DateTime.Today.AddMonths(-12),
                        Ref = "123/456789"
                    },
                    new PayeSchemeModel
                    {
                        AddedDate = DateTime.Today.AddMonths(-12),
                        Ref = "124/AA6789"
                    }
                }
            };
            var challengePermissionQuery = new ChallengePermissionQuery
            {
                Id = "123",
                Balance = "£Z000",
                ChallengeElement1 = "1",
                ChallengeElement2 = "A",
                FirstCharacterPosition = 0,
                SecondCharacterPosition = 4
            };

            var balance = 1000m;

            _accountRepository.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
                .ReturnsAsync(balance);

            var actual = await _unit.CheckData(account, challengePermissionQuery);

            Assert.IsFalse(actual);
        }

        [Test]
        public async Task ItShouldReturnFalseWhenCheckDataHasInvalidCharacterData()
        {
            var account = new Core.Models.Account
            {
                Transactions = new List<TransactionViewModel>
                {
                    new TransactionViewModel {Balance = 300m},
                    new TransactionViewModel {Balance = 700m}
                },
                PayeSchemes = new List<PayeSchemeModel>
                {
                    new PayeSchemeModel
                    {
                        AddedDate = DateTime.Today.AddMonths(-12),
                        Ref = "123/456789"
                    },
                    new PayeSchemeModel
                    {
                        AddedDate = DateTime.Today.AddMonths(-12),
                        Ref = "124/456789"
                    }
                }
            };
            var challengePermissionQuery = new ChallengePermissionQuery
            {
                Id = "123",
                Balance = "£1000",
                ChallengeElement1 = "1",
                ChallengeElement2 = "A",
                FirstCharacterPosition = 0,
                SecondCharacterPosition = 1
            };

            var balance = 1000m;

            _accountRepository.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
                .ReturnsAsync(balance);

            var actual = await _unit.CheckData(account, challengePermissionQuery);

            Assert.IsFalse(actual);
        }

        [Test]
        public async Task ItShouldReturnTrueWhenCheckDataHasValidData()
        {
            var account = new Core.Models.Account
            {
                Transactions = new List<TransactionViewModel>
                {
                    new TransactionViewModel {Balance = 300m},
                    new TransactionViewModel {Balance = 700m}
                },
                PayeSchemes = new List<PayeSchemeModel>
                {
                    new PayeSchemeModel
                    {
                        AddedDate = DateTime.Today.AddMonths(-12),
                        Ref = "123/456789"
                    },
                    new PayeSchemeModel
                    {
                        AddedDate = DateTime.Today.AddMonths(-12),
                        Ref = "124/45A789"
                    }
                }
            };
            var challengePermissionQuery = new ChallengePermissionQuery
            {
                Id = "123",
                Balance = "£1000",
                ChallengeElement1 = "1",
                ChallengeElement2 = "A",
                FirstCharacterPosition = 0,
                SecondCharacterPosition = 5
            };

            var balance = 1000m;

            _accountRepository.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
                .ReturnsAsync(balance);

            var actual = await _unit.CheckData(account, challengePermissionQuery);

            Assert.IsTrue(actual);
        }
    }
}