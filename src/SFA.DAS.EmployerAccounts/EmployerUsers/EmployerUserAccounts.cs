using SFA.DAS.EmployerAccounts.EmployerUsers.ApiResponse;

namespace SFA.DAS.EmployerAccounts.EmployerUsers;

public class EmployerUserAccounts
{
    public string Email { get; set; }
    public string EmployerUserId { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public IEnumerable<EmployerUserAccountItem> EmployerAccounts { get; set; }

    public static implicit operator EmployerUserAccounts(GetUserAccountsResponse source)
    {
        if (source?.UserAccounts == null)
        {
            return new EmployerUserAccounts
            {
                FirstName = source?.FirstName,
                LastName = source?.LastName,
                EmployerUserId = source?.EmployerUserId,
                Email = source?.Email,
                EmployerAccounts = new List<EmployerUserAccountItem>()
            };
        }

        return new EmployerUserAccounts
        {
            FirstName = source.FirstName,
            LastName = source.LastName,
            EmployerUserId = source.EmployerUserId,
            Email = source.Email,
            EmployerAccounts = source.UserAccounts.Select(c => (EmployerUserAccountItem)c).ToList()
        };
    }
}

public class EmployerUserAccountItem
{
    public string AccountId { get; set; }
    public string EmployerName { get; set; }
    public string Role { get; set; }

    public static implicit operator EmployerUserAccountItem(EmployerIdentifier source)
    {
        return new EmployerUserAccountItem
        {
            AccountId = source.AccountId,
            EmployerName = source.EmployerName,
            Role = source.Role
        };
    }
}