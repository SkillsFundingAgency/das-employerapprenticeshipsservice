namespace SFA.DAS.EmployerAccounts.Models.Reservations;

public class Reservation
{
    public Guid Id { get; set; }
    public long AccountId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public Course Course { get; set; }
    public ReservationStatus Status { get; set; }
    public long AccountLegalEntityId { get; set; }
    public string AccountLegalEntityName { get; set; }
}

public enum ReservationStatus
{
    Pending = 0,
    Confirmed = 1,
    Completed = 2,
    Deleted = 3
}