﻿using MediatR;

namespace SFA.DAS.EmployerAccounts.Commands.AddPayeToAccount
{
    public class AddPayeToAccountCommand : IAsyncRequest
    {
        public string ExternalUserId { get; set; }
        public string Empref { get; set; }
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public string HashedAccountId { get; set; }
        public string EmprefName { get; set; }
    }
}
