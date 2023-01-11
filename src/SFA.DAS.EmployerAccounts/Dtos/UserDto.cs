namespace SFA.DAS.EmployerAccounts.Dtos;

public class UserDto
{
    public long Id { get; set; }
    public Guid Ref { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}