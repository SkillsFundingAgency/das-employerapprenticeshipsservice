namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IEmpRefFileBasedService
{
    Task<string> GetEmpRef(string email, string id);
}