﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Authorization
{
    public interface IAuthorizationContextCache
    {
        IAuthorizationContext GetAuthorizationContext();
        void SetAuthorizationContext(IAuthorizationContext authorizationContext);
    }
}
