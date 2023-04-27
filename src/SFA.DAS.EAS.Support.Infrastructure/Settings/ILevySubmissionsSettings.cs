namespace SFA.DAS.EAS.Support.Infrastructure.Settings;

public interface ILevySubmissionsSettings
{
    TokenServiceApiClientConfiguration TokenServiceApi { get; set; }

    HmrcApiClientConfiguration HmrcApi { get; set; }

}
