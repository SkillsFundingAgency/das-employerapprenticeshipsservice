using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.TestCommon.DbCleanup
{
    public interface IUpdateTransactionLine
    {
        Task Execute(long submissionId, DateTime createdDate);
    }
}