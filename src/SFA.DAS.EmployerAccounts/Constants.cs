﻿namespace SFA.DAS.EmployerAccounts
{
    public static class Constants
    {
        public const string AccountHashedIdRegex = @"^[A-Za-z\d]{6,6}$";
        public const string ServiceName = "SFA.DAS.EmployerAccounts";
        public const string Tier2User = "Tier2User";

        public const string ServiceNamespace = "SFA.DAS.EmployerAccounts";
        public struct TransferConnectionInvitations
        {
            public const decimal SenderMinTransferAllowance = 1;
        }

        public const string DefaultServiceTimeout = "DefaultServiceTimeout";
    }
}