namespace SFA.DAS.EAS.Domain
{
    public static class Constants
    {
        public const string AccountHashedIdRegex = @"^[A-Za-z\d]{6,6}$";
        public const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";
        public const string ServiceNamespace = "SFA.DAS.EAS";
        public const string ServiceVersion = "1.0";

        public struct TransferConnectionInvitations
        {
            public const int SenderMaxTransferConnectionInvitations = 1;
            public const decimal SenderMinTransferAllowance = 1;
        }
    }
}