using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Caching
{
    public interface ICacheProvider
    {
        T Get<T>(string key);
        void Set(string key, object value, TimeSpan slidingExpiration);
        void Set(string key, object value, DateTimeOffset absoluteExpiration);
    }
}
