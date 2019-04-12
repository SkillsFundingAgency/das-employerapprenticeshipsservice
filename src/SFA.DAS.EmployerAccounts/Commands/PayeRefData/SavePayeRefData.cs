using System;
using MediatR;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Commands.PayeRefData
{
    public sealed class SavePayeRefData : IAsyncRequest
    {
        public SavePayeRefData(EmployerAccountPayeRefData payeRefData)
        {
            PayeRefData = payeRefData ?? throw new ArgumentNullException(nameof(payeRefData));
        }

        public EmployerAccountPayeRefData PayeRefData { get;  }
    }
}