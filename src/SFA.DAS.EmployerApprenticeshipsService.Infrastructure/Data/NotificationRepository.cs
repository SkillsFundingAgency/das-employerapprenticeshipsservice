using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class NotificationRepository : BaseRepository, INotificationRepository
    {
        public NotificationRepository(EmployerApprenticeshipsServiceConfiguration configuration) : base(configuration)
        {
        }

        public async Task<int> Create(NotificationMessage message)
        {
            return await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", message.UserId, DbType.Int32);
                parameters.Add("@DateTime", message.DateTime, DbType.DateTime);
                parameters.Add("@ForceFormat", message.ForceFormat, DbType.Boolean);
                parameters.Add("@TemplateId", message.TemplatedId, DbType.String);
                parameters.Add("@Data", message.Data, DbType.String);
                parameters.Add("@MessageFormat", message.MessageFormat, DbType.Int16);
                parameters.Add("@Id", null, DbType.Int32, ParameterDirection.Output, 4);

                await c.ExecuteAsync("[dbo].[CreateNotification]", parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("@Id");
            });
        }
    }
}
