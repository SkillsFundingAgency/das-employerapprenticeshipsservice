namespace SFA.DAS.EmployerAccounts.Dtos;

public class AccountDto
{
    public long Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string HashedId { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string Name { get; set; }
    public string PublicHashedId { get; set; }
}