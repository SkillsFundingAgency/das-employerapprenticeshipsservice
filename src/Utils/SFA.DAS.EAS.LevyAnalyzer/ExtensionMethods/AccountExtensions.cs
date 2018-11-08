using System.Linq;
using SFA.DAS.EAS.LevyAnalyser.Models;

namespace SFA.DAS.EAS.LevyAnalyser.ExtensionMethods
{
    public static class AccountExtensions
    {
        /// <summary>
        ///     Separate out the account into it's constituent employers.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static Employer[] SeperateEmployers(this Account account)
        {
            return account.LevyDeclarations.Select(declaration => declaration.EmpRef)
                .Union(account.Transactions.Select(transaction => transaction.EmpRef))
                .Distinct()
                .OrderBy(empRef => empRef)
                .Select(empRef => new Employer(
                    empRef,
                    account.LevyDeclarations.Where(declaration => declaration.EmpRef == empRef),
                    account.Transactions.Where(transaction => transaction.EmpRef == empRef)))
                .ToArray();
        }
    }
}