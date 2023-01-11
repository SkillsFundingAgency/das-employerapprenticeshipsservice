using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;

public class GetEmployerAccountDetailByHashedIdQuery : IAsyncRequest<GetEmployerAccountDetailByHashedIdResponse>
{
    [Required]
    public string HashedAccountId { get; set; }
}