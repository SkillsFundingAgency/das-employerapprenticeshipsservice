﻿using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.FindEmployerAccountLevyDeclarationTransactions
{
    /// <summary>
    ///  AML-2454: Move to finance
    /// </summary>
    public class FindEmployerAccountLevyDeclarationTransactionsQuery : IAsyncRequest<FindEmployerAccountLevyDeclarationTransactionsResponse>
    {
        public string HashedAccountId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ExternalUserId { get; set; }
    }
}
