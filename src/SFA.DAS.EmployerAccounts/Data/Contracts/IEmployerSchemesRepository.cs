using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Data.Contracts;

public interface IEmployerSchemesRepository
{
    Task<PayeScheme> GetSchemeByRef(string empref);
}