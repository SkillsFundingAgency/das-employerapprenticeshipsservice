using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.ReadStore.Mediator
{
    internal class ApiMediator : IApiMediator
    {
        private static readonly ConcurrentDictionary<Type, object> RequestHandlers = new ConcurrentDictionary<Type, object>();
        
        private readonly ApiServiceFactory _serviceFactory;

        public ApiMediator(ApiServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public Task<TResponse> Send<TResponse>(IApiRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestType = request.GetType();
            
            var handler = (RequestHandler<TResponse>)RequestHandlers.GetOrAdd(requestType, t =>
            {
                var responseType = typeof(TResponse);
                var handlerOpenGenericType = typeof(RequestHandler<,>);
                var handlerClosedGenericType = handlerOpenGenericType.MakeGenericType(requestType, responseType);
                var handlerInstance = Activator.CreateInstance(handlerClosedGenericType);

                return handlerInstance;
            });

            return handler.Handle(request, cancellationToken, _serviceFactory);
        }
        
        private abstract class RequestHandler<TResponse>
        {
            public abstract Task<TResponse> Handle(IApiRequest<TResponse> request, CancellationToken cancellationToken, ApiServiceFactory serviceFactory);
        }
    
        private class RequestHandler<TRequest, TResponse> : RequestHandler<TResponse> where TRequest : IApiRequest<TResponse>
        {
            public override Task<TResponse> Handle(IApiRequest<TResponse> request, CancellationToken cancellationToken, ApiServiceFactory serviceFactory)
            {
                var handler = serviceFactory.GetInstance<IApiRequestHandler<TRequest, TResponse>>();
            
                return handler.Handle((TRequest)request, cancellationToken);
            }
        }
    }
}