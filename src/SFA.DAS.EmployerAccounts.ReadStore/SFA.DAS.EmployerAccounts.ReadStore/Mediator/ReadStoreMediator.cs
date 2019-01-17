using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.ReadStore.Mediator
{
    internal class ReadStoreMediator : IReadStoreMediator
    {
        private static readonly ConcurrentDictionary<Type, object> RequestHandlers = new ConcurrentDictionary<Type, object>();
        
        private readonly ReadStoreServiceFactory _serviceFactory;

        public ReadStoreMediator(ReadStoreServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public Task<TResponse> Send<TResponse>(IReadStoreRequest<TResponse> request, CancellationToken cancellationToken = default)
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
            public abstract Task<TResponse> Handle(IReadStoreRequest<TResponse> request, CancellationToken cancellationToken, ReadStoreServiceFactory serviceFactory);
        }
    
        private class RequestHandler<TRequest, TResponse> : RequestHandler<TResponse> where TRequest : IReadStoreRequest<TResponse>
        {
            public override Task<TResponse> Handle(IReadStoreRequest<TResponse> request, CancellationToken cancellationToken, ReadStoreServiceFactory serviceFactory)
            {
                var handler = serviceFactory.GetInstance<IReadStoreRequestHandler<TRequest, TResponse>>();
            
                return handler.Handle((TRequest)request, cancellationToken);
            }
        }
    }
}