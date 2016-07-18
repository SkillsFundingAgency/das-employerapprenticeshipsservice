namespace SFA.DAS.LevyAggregationProvider.Worker
{
    public class LevyAggregationConfiguration
    {
        public EmployerConfiguration Employer { get; set; }
    }

    public class EmployerConfiguration
    {
        public string DatabaseConnectionString { get; set; }
    }
}