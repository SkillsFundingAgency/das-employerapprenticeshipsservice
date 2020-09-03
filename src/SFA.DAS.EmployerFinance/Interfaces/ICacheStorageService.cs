using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Interfaces
{
    public interface ICacheStorageService
    {
        Task Save<T>(string key, T item, int expirationInMinutes);
        Task Delete(string key);
        bool TryGet(string key, out string value);
    }
}
