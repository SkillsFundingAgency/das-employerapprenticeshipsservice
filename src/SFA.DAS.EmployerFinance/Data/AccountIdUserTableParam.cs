using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SFA.DAS.EmployerFinance.Data
{
    public class AccountIdUserTableParam : SqlMapper.IDynamicParameters
    {
        private ICollection<SqlParameter> _additionalParameters { get; }

        readonly IEnumerable<long> _accountIds;
        public AccountIdUserTableParam(IEnumerable<long> accountIds)
        {
            _accountIds = accountIds;
            _additionalParameters = new List<SqlParameter>();
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            var sqlCommand = (SqlCommand)command;
            sqlCommand.CommandType = CommandType.StoredProcedure;

            var accountList = new List<Microsoft.SqlServer.Server.SqlDataRecord>();

            Microsoft.SqlServer.Server.SqlMetaData[] tvpDefinition = { new Microsoft.SqlServer.Server.SqlMetaData("AccountId", SqlDbType.BigInt) };

            foreach (var accountId in _accountIds)
            {
                Microsoft.SqlServer.Server.SqlDataRecord rec = new Microsoft.SqlServer.Server.SqlDataRecord(tvpDefinition);
                rec.SetInt64(0, accountId);
                accountList.Add(rec);
            }

            var p = sqlCommand.Parameters.Add("@accountIds", SqlDbType.Structured);
            p.Direction = ParameterDirection.Input;
            p.TypeName = "[employer_financial].[AccountIds]";
            p.Value = accountList;

            sqlCommand.Parameters.AddRange(_additionalParameters.ToArray());
        }

        public void Add(string parameterName, object parameterValue, DbType parameterType)
        {
            var parameter = new SqlParameter(parameterName, parameterType) { Value = parameterValue };

            _additionalParameters.Add(parameter);
        }
    }
}
