using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Adapters;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Dtos;
using SFA.DAS.Hashing;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper
{
    class DbBuilder
    {
        private readonly IHashingService _hashingService;
        private readonly IPublicHashingService _publicHashingService;

        public DbBuilder(
            DbBuilderDependentRepositories dependentRepositories,
            IHashingService hashingService,
            IPublicHashingService publicHashingService)
        {
            DependentRepositories = dependentRepositories;
            _hashingService = hashingService;
            _publicHashingService = publicHashingService;
        }

        public DbBuilderContext Context { get;  } = new DbBuilderContext();

        public DbBuilderDependentRepositories DependentRepositories { get; }

        public DbBuilder EnsureAccountExists(EmployerAccountInput input)
        {
            return EnsureAccountExists(input, CancellationToken.None);
        }

        public DbBuilder EnsureAccountExists(EmployerAccountInput input, CancellationToken cancellationToken)
        {
            return WaitDbAction(EnsureAccountExistsAsync(input), cancellationToken);
        }

        public async Task<DbBuilder> EnsureAccountExistsAsync(EmployerAccountInput input)
        {
            var output = await DependentRepositories.DbDirectRepository.GetAccountDetailsAsync(input.OrganisationName);

            if (output == null)
            {
                await DependentRepositories.AccountRepository.CreateAccount(
                    input.UserId,
                    input.OrganisationReferenceNumber,
                    input.OrganisationName,
                    input.OrganisationRegisteredAddress,
                    input.OrganisationDateOfInception,
                    input.PayeReference,
                    input.AccessToken,
                    input.RefreshToken,
                    input.OrganisationStatus,
                    input.EmployerRefName,
                    input.Source,
                    input.PublicSectorDataSource,
                    input.Sector);

                output = await DependentRepositories.DbDirectRepository.GetAccountDetailsAsync(input.OrganisationName);
            }

            if (string.IsNullOrWhiteSpace(output.HashedAccountId))
            {
                output.HashedAccountId = _hashingService.HashValue(output.AccountId);
                output.PublicHashedAccountId = _publicHashingService.HashValue(output.AccountId);
                await DependentRepositories.AccountRepository.UpdateAccountHashedIds(output.AccountId, output.HashedAccountId, output.PublicHashedAccountId);
            }

            Context.ActiveEmployerAccount = output;

            return this;
        }

        public DbBuilder WithLegalEntity(LegalEntityWithAgreementInput input)
        {
            return WithLegalEntity(input, CancellationToken.None);
        }

        public DbBuilder WithLegalEntity(LegalEntityWithAgreementInput input, CancellationToken cancellationToken)
        {
            return WaitDbAction(WithLegalEntityAsync(input), cancellationToken);
        }

        public async Task<DbBuilder> WithLegalEntityAsync(LegalEntityWithAgreementInput input)
        {
            var output = new LegalEnityWithAgreementOutput();

            var view = await DependentRepositories.AccountRepository.CreateLegalEntity(
                input.AccountId,
                new LegalEntityWithAgreementInputToLegalEntityAdapter(input));

            output.EmployerAgreementId = view.Id;
            output.LegalEntityId = view.LegalEntityId;
            output.HashedAgreementId = view.HashedAgreementId;

            return this;
        }

        public DbBuilder EnsureUserExists(UserInput input)
        {
            return EnsureUserExists(input, CancellationToken.None);
        }

        public DbBuilder EnsureUserExists(UserInput input, CancellationToken cancellationToken)
        {
            return WaitDbAction(EnsureUserExistsAsync(input), cancellationToken);
        }

        public async Task<DbBuilder> EnsureUserExistsAsync(UserInput input)
        {
            var output = new UserOutput();

            await DependentRepositories.UserRepository.Upsert(new UserInputToUserAdapter(input));
            var user = await DependentRepositories.UserRepository.GetUserByRef(input.UserRef);
            output.UserRef = input.UserRef;
            output.UserId = user.Id;
            Context.ActiveUser = output;
            return this;
        }

        private TResult WaitDbAction<TResult>(Task<TResult> action, CancellationToken cancellationToken)
        {
            action.Wait((int) TestConstants.DbTimeout.TotalMilliseconds, cancellationToken);
            CheckTaskResult(action);
            return action.Result;
        }

        private void CheckTaskResult(Task action)
        {
            var failMessage = new StringBuilder();

            CheckIfTaskCanceled(action, failMessage);

            CheckIfTaskFaulted(action, failMessage);

            if (failMessage.Length > 0)
            {
                Assert.Fail(failMessage.ToString());
            }
        }

        private static void CheckIfTaskFaulted(Task action, StringBuilder failMessage)
        {
            if (action.IsFaulted)
            {
                failMessage.Append("A DB task has faulted. ");
                if (action.Exception != null)
                {
                    foreach (var exception in action.Exception.Flatten().InnerExceptions)
                    {
                        failMessage.AppendLine($">>:e => {exception.GetType().Name} : {exception.Message}");
                    }
                }
            }
        }

        private static void CheckIfTaskCanceled(Task action, StringBuilder failMessage)
        {
            if (action.IsCanceled)
            {
                failMessage.AppendLine(
                    $"A DB task has been cancelled, possibly because it has timed out. Timeout value is: {TestConstants.DbTimeout}");
            }
        }
    }
}