namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public interface IAccountViewModel
{
    long AccountId { get; set; }
    string AccountHashedId { get; set; }
}