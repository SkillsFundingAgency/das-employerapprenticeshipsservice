using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models
{
    public interface IEntity
    {
        IEnumerable<object> GetEvents();
    }
}