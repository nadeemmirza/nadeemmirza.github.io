using System.Collections.Concurrent;
using System.Reflection;
using EmailTemplateApi.Application.Common.Messaging;

namespace EmailTemplateApi.Infrastructure.Mediation;

public sealed class SimpleMediator(IServiceProvider serviceProvider) : IMediator
{
    private static readonly ConcurrentDictionary<(Type RequestType, Type ResponseType), MethodInfo> DispatchMethods = new();

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var dispatchMethod = DispatchMethods.GetOrAdd(
            (request.GetType(), typeof(TResponse)),
            static key => typeof(SimpleMediator)
                .GetMethod(nameof(DispatchAsync), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(key.RequestType, key.ResponseType));

        return (Task<TResponse>)dispatchMethod.Invoke(null, [serviceProvider, request, cancellationToken])!;
    }

    private static Task<TResponse> DispatchAsync<TRequest, TResponse>(
        IServiceProvider serviceProvider,
        IRequest<TResponse> request,
        CancellationToken cancellationToken)
        where TRequest : IRequest<TResponse>
    {
        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        return handler.Handle((TRequest)request, cancellationToken);
    }
}
