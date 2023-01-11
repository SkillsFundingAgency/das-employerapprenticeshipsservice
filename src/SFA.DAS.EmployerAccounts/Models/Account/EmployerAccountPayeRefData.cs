namespace SFA.DAS.EmployerAccounts.Models.Account;

public class EmployerAccountPayeRefData
{
    public string PayeReference { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public bool EmpRefNotFound { get; set; }
    public string EmployerRefName { get; set; }
    public string AORN { get; set; }
}