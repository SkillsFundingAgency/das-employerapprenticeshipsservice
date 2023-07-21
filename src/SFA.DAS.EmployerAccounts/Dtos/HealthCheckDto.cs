namespace SFA.DAS.EmployerAccounts.Dtos;

public class HealthCheckDto
{
    public int Id { get; set; }
    public DateTime SentRequest { get; set; }
    public DateTime? ReceivedResponse { get; set; }
    public DateTime PublishedEvent { get; set; }
    public DateTime? ReceivedEvent { get; set; }
}