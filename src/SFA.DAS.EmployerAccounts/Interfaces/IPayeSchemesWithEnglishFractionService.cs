using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IPayeSchemesWithEnglishFractionService
{
    Task<IEnumerable<PayeView>> GetPayeSchemes(long accountId);
}