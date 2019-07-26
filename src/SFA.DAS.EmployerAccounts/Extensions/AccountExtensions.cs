using SFA.DAS.EmployerAccounts.Models.Portal;

namespace SFA.DAS.EmployerAccounts.Extensions
{
    public static class AccountExtensions
    {
        public static Cardinality GetVacancyCardinality(this EAS.Portal.Client.Types.Account account)
        {
            switch (account.Vacancies.Count)
            {
                case 0:
                    return Cardinality.None;
                case 1:
                    return Cardinality.One;
                default:
                    return Cardinality.Many;
            }
        }
    }
}