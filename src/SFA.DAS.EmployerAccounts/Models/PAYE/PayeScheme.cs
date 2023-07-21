namespace SFA.DAS.EmployerAccounts.Models.PAYE;

public class PayeScheme
{
    public int Id { get; set; }
    public string Ref { get; set; }
    public long AccountId { get; set; }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public DateTime AddedDate { get; set; }
    public DateTime? RemovedDate { get; set; }
}