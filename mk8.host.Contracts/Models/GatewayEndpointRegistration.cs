using System;
using System.Collections.Generic;
using System.Text;

namespace mk8.host.Contracts.Models
{
    public sealed record GatewayEndpointRegistration
    {
        public required Guid Id { get; init; }
        public int Order { get; init; }
        public required string Service { get; init; }
        public required string Name { get; init; }
        public required string RoutePattern { get; init; }
        public required IReadOnlyList<string> HttpMethods { get; init; }
    }
}
