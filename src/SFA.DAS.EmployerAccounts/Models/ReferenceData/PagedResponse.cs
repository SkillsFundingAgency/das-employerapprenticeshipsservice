namespace SFA.DAS.EmployerAccounts.Models.ReferenceData;

public class PagedResponse<T>
{
    public ICollection<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalResults { get; set; }
}