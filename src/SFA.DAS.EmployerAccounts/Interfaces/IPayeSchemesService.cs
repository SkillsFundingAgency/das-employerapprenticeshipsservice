using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IPayeSchemesService
{
    Task<IEnumerable<PayeView>> GetPayeSchemes(long accountId);
}