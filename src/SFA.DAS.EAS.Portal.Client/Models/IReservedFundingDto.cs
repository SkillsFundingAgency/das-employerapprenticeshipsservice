using System;

namespace SFA.DAS.EAS.Portal.Client.Models
{
    public interface IReservedFundingDto
    {
        Guid ReservationId { get;}
        //optional
        string CourseId { get; }
        //optional
        string CourseName { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
    }
}