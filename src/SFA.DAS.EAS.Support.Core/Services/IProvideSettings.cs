namespace SFA.DAS.EAS.Support.Core.Services
{
    public interface IProvideSettings
    {
        string GetSetting(string settingKey);
        string GetNullableSetting(string settingKey);
    }
}