using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.Documents;
using SFA.DAS.EmployerAccounts.ReadStore.Data;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;

internal class PingQueryHandler : IRequestHandler<PingQuery>
{
    private readonly IDocumentClient _documentClient;

    public PingQueryHandler(IDocumentClient documentClient)
    {
        _documentClient = documentClient;
    }

    public Task<Unit> Handle(PingQuery request, CancellationToken cancellationToken)
    {
        var value = _documentClient.CreateDatabaseQuery()
            .Where(d => d.Id == DocumentSettings.DatabaseName)
            .Select(d => 1)
            .AsEnumerable()
            .FirstOrDefault();

        if (value == 0)
        {
            throw new PingException();
        }

        return Task.FromResult(Unit.Value);
    }
}

[Serializable]
public class PingException : SystemException
{
    public PingException() : base("Read store database ping failed") { }
}