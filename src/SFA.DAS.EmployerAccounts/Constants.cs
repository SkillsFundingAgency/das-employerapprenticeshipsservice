﻿namespace SFA.DAS.EmployerAccounts
{
    public static class Constants
    {
        public const string AccountHashedIdRegex = @"^[A-Za-z\d]{6,6}$";
        public const string ServiceName = "SFA.DAS.EmployerAccounts";

        public struct TransferConnectionInvitations
        {
            public const decimal SenderMinTransferAllowance = 1;
        }
    }
}