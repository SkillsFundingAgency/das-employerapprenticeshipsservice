using System.Data.Common;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public static class DbConnectionExtensions
    {
        public static async Task TryOpenAsync(this DbConnection connection)
        {
            try
            {
                await connection.OpenAsync().ConfigureAwait(false);
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }
    }
}