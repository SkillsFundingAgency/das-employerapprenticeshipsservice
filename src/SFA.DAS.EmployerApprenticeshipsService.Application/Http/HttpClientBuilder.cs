using System.Net.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Http.MessageHandlers;

namespace SFA.DAS.EAS.Application.Http;

public sealed class HttpClientBuilder
{
    private DelegatingHandler _rootHandler;

    private DelegatingHandler _leafHandler;

    public HttpClient Build()
    {
        if (_rootHandler == null)
        {
            return new HttpClient();
        }

        _leafHandler.InnerHandler = new HttpClientHandler();
        return new HttpClient(_rootHandler);
    }

    public HttpClientBuilder WithDefaultHeaders()
    {
        DefaultHeadersHandler newHandler = new DefaultHeadersHandler();
        AddHandlerToChain(newHandler);
        return this;
    }

    public HttpClientBuilder WithManagedIdentityAuthorisationHeader(IManagedIdentityTokenGenerator tokenGenerator)
    {
        ManagedIdentityHeadersHandler newHandler = new(tokenGenerator);
        AddHandlerToChain(newHandler);
        return this;
    }

    public HttpClientBuilder WithLogging(ILoggerFactory loggerFactory)
    {
        LoggingMessageHandler newHandler = new(loggerFactory.CreateLogger<LoggingMessageHandler>());
        AddHandlerToChain(newHandler);
        return this;
    }

    private void AddHandlerToChain(DelegatingHandler newHandler)
    {
        if (_rootHandler == null)
        {
            _rootHandler = newHandler;
            _leafHandler = newHandler;
        }
        else
        {
            _leafHandler.InnerHandler = newHandler;
            _leafHandler = newHandler;
        }
    }
}
