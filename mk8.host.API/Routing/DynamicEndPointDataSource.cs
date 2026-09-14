using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Primitives;
using mk8.host.Contracts.Models;

namespace mk8.host.Gateway.Routing
{
    public sealed class DynamicEndPointDataSource : EndpointDataSource
    {
        private readonly object _lock = new();

        private IReadOnlyList<Endpoint> _endpoints = Array.Empty<Endpoint>();
        private CancellationTokenSource _changeTokenSource = new();

        public override IReadOnlyList<Endpoint> Endpoints
        {
            get
            {
                lock (_lock)
                {
                    return _endpoints;
                }
            }
        }

        public override IChangeToken GetChangeToken()
        {
            lock (_lock)
            {
                return new CancellationChangeToken(_changeTokenSource.Token);
            }
        }

        private static Endpoint BuildEndpoint(GatewayEndpointRegistration registration, RequestDelegate requestDelegate)
        {
            var routePattern = RoutePatternFactory.Parse(registration.RoutePattern);

            var builder = new RouteEndpointBuilder(requestDelegate, routePattern, registration.Order)
            {
                DisplayName = $"mk8.host:{registration.Service}:{registration.Name}"
            };

            builder.Metadata.Add(registration);

            builder.Metadata.Add(new HttpMethodMetadata(registration.HttpMethods));

            return builder.Build();
        }

        public void Replace(IReadOnlyCollection<GatewayEndpointRegistration> registrations, RequestDelegate requestDelegate)
        {
            var endpoints = registrations.Select(i => BuildEndpoint(i, requestDelegate)).ToArray();

            CancellationTokenSource previousChangeTokenSource;

            lock (_lock)
            {
                _endpoints = endpoints;

                previousChangeTokenSource = _changeTokenSource;
                _changeTokenSource = new CancellationTokenSource();

            }

            previousChangeTokenSource.Cancel();
            previousChangeTokenSource.Dispose();
        }
    }
}
