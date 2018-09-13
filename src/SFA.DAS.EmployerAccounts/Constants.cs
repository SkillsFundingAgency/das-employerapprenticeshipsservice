namespace SFA.DAS.EmployerAccounts
{
    public static class Constants
    {
        public const string AccountHashedIdRegex = @"^[A-Za-z\d]{6,6}$";

        public struct TransferConnectionInvitations
        {
            public const decimal SenderMinTransferAllowance = 1;
        }
    }
}