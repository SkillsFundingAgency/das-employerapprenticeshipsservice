namespace SFA.DAS.EmployerAccounts.Web.Models;

public class TrainingDateModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public override string ToString()
    {
        return EndDate == default(DateTime) ?
            $"{StartDate:MMMM yyyy}" :
            $"Between {StartDate:MMMM yyyy} and {EndDate:MMMM yyyy}";
    }

    public override bool Equals(object obj)
    {
        if (!(obj is TrainingDateModel targetModel)) return false;

        return StartDate.Year.Equals(targetModel.StartDate.Year) &&
               StartDate.Month.Equals(targetModel.StartDate.Month) &&
               StartDate.Day.Equals(targetModel.StartDate.Day) &&
               EndDate.Year.Equals(targetModel.EndDate.Year) &&
               EndDate.Month.Equals(targetModel.EndDate.Month) &&
               EndDate.Day.Equals(targetModel.EndDate.Day);
    }

    public override int GetHashCode()
    {
        return -1;
    }
}