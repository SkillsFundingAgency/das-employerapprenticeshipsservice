using System;

namespace SFA.DAS.EAS.Portal.Client.Models
{
    public interface IReservedFundingDto
    {
        long ReservationId { get; }
        string LegalEntityName { get; }
        string CourseName { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
    }
}