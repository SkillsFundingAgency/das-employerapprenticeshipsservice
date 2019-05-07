using System;

namespace SFA.DAS.EAS.Portal.Client.Models
{
    public interface IReservedFundingDto
    {
        long ReservationId { get;}
        //optional
        long CourseId { get; }
        //optional
        string CourseName { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
    }
}