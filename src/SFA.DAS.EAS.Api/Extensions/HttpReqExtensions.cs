using Microsoft.AspNetCore.Http;
using System.Net;

namespace SFA.DAS.EAS.Account.Api.Extensions;

public static class HttpReqExtensions
{
    public static bool IsLocal(this HttpContext req)
    {
        var connection = req.Connection;
        if (connection.RemoteIpAddress != null)
        {
            if (connection.LocalIpAddress != null)
            {
                return connection.RemoteIpAddress.Equals(connection.LocalIpAddress);
            }
            else
            {
                return IPAddress.IsLoopback(connection.RemoteIpAddress);
            }
        }

        // for in memory TestServer or when dealing with default connection info
        if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
        {
            return true;
        }

        return false;
    }
}
