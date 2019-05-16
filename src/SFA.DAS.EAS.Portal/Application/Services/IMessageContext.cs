using System;

namespace SFA.DAS.EAS.Portal.Application.Services
{
    public interface IMessageContext
    {
        string Id { get; set; }
        DateTime CreatedDateTime { get; set; }
    }
}
