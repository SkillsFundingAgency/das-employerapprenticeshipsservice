using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Jobs.RunOnceJobs;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.TestCommon;
using SFA.DAS.EmployerAccounts.TestCommon.DatabaseMock;

namespace SFA.DAS.EmployerAccounts.Jobs.UnitTests.OneOffJobs
{
    [TestFixture]
    [Parallelizable]
    public class RunOnceServiceTests : Testing.FluentTest<RunOnceServiceTestsFixture>
    {
        [Test]
        public Task RunOnce_WhenRunningAfterJobHasPreviouslySuccessfullyCompleted_ThenShouldImmediatelyReturnAndNotRunTheJob()
        {
            return TestAsync(f => f.SetJobAsAlreadyRun(),
                f => f.Run(), f => f.VerifyJobWasNotAddedAgain());
        }

        [Test]
        public Task RunOnce_WhenRunningJobFirstTime_ThenShouldCallOneTimeJob()
        {
            return TestAsync(f => f.Run(), f => f.VerifyOneOffJobWasCalled());
        }

        [Test]
        public Task RunOnce_WhenRunningJobFirstTime_ThenShouldMarkJobAsPopulated()
        {
            return TestAsync(f => f.Run(), f => f.VerifyJobWasAdded());
        }

    }

    public class RunOnceServiceTestsFixture
    {
        internal Mock<EmployerAccountsDbContext> EmployerAccountsDbContext { get; set; }
        public Mock<ILogger<RunOnceJobsService>> Logger { get; set; }

        internal RunOnceJobsService RunOnceJobsService { get; set; }

        private bool _functionWasCalled = false;
        private readonly string _jobName = typeof(SeedAccountUsersJob).Name;

        private readonly List<RunOnceJob> _jobsList = new List<RunOnceJob>();

        public RunOnceServiceTestsFixture()
        {
            EmployerAccountsDbContext = new Mock<EmployerAccountsDbContext>();
            EmployerAccountsDbContext.Setup(x => x.RunOnceJobs).Returns(_jobsList.AsQueryable().BuildMockDbSet().Object);

            Logger = new Mock<ILogger<RunOnceJobsService>>();

            RunOnceJobsService = new RunOnceJobsService(new Lazy<EmployerAccountsDbContext>(() => EmployerAccountsDbContext.Object), Logger.Object);
        }

        public Task Run()
        {
            return RunOnceJobsService.RunOnce(_jobName, () =>
            {
                _functionWasCalled = true;
                return Task.CompletedTask;
            });
        }

        public RunOnceServiceTestsFixture SetJobAsAlreadyRun()
        {
            _jobsList.Add(new RunOnceJob(_jobName, DateTime.UtcNow));
            return this;
        }

        public RunOnceServiceTestsFixture VerifyJobWasAdded()
        {
            var jobs = EmployerAccountsDbContext.Object.RunOnceJobs;
            if (jobs.Count(x => x.Name == _jobName) == 1)
            {
                throw new Exception($"Expecting '{_jobName}' to have been added");
            }

            return this;
        }

        public RunOnceServiceTestsFixture VerifyOneOffJobWasCalled()
        {
            if (!_functionWasCalled)
            {
                throw new Exception("Function was not called");
            }

            return this;
        }

        public RunOnceServiceTestsFixture VerifyJobWasNotAddedAgain()
        {
            var jobs = EmployerAccountsDbContext.Object.RunOnceJobs;
            if (jobs.Count(x => x.Name == _jobName) != 1)
            {
                throw new Exception($"Expecting '{_jobName}' to have been added again");
            }

            return this;
        }
    }
}