namespace SFA.DAS.EAS.Domain.Models.Settings
{
    public class UserNotificationSetting
    {
        public long EmployerAgreementId { get; set; }
        public long UserId { get; set; }
        public string LegalEntityName { get; set; }
        public bool ReceiveNotifications { get; set; }
    }
}
