﻿using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountByHashedId
{
    public class GetEmployerAccountByHashedIdResponse
    {
        public AccountDetail Account { get; set; }
    }
}