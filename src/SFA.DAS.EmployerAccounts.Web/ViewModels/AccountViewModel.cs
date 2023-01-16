using AutoMapper;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public abstract class AccountViewModel : IAccountViewModel
{
    [IgnoreMap]
    public long AccountId { get; set; }

    [IgnoreMap]
    public string AccountHashedId { get; set; }
}