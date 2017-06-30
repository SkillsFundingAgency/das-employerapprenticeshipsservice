namespace SFA.DAS.EAS.Web.ViewModels.Interfaces
{
    public interface IPaginationViewModel
    {
        int PageNumber { get; }

        int TotalPages { get; }

        int PageSize { get; }

        int TotalResults { get; }
    }
}