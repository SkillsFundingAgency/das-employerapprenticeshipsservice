using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Models.PAYE;

public class Paye
{
    [Key]
    public string EmpRef { get; set; }
    public long AccountId { get; set; }  

    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string RefName { get; set; }
    public string Aorn { get; set; }
}