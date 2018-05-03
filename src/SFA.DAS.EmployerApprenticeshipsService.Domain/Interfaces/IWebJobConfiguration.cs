namespace SFA.DAS.EAS.Domain.Interfaces
{
	public interface IWebJobConfiguration
	{
		string DashboardConnectionString { get; set; }
		string StorageConnectionString { get; set; }
	}
}