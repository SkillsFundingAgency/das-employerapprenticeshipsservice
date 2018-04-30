﻿using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccount
{
    public class GetEmployerAccountHashedQuery : IAsyncRequest<GetEmployerAccountResponse>
    {
        public string HashedAccountId { get; set; }

        public Guid ExternalUserId { get; set; }
    }
}
