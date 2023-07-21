using AutoMapper.Configuration.Annotations;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public abstract class AccountViewModel : IAccountViewModel
{
    [Ignore]
    public long AccountId { get; set; }

    [Ignore]
    public string AccountHashedId { get; set; }
}