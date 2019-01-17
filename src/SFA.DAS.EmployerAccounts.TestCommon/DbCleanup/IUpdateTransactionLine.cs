using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.TestCommon.DbCleanup
{
    public interface IUpdateTransactionLine
    {
        Task Execute(long submissionId, DateTime createdDate);
    }
}