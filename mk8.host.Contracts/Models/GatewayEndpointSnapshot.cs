using System;
using System.Collections.Generic;
using System.Text;

namespace mk8.host.Contracts.Models
{
    public sealed record GatewayEndpointSnapshot
    {
        public required Guid Id { get; init; }
        public required long Revision { get; init; }
        public required IReadOnlyList<GatewayEndpointRegistration> Endpoints { get; init; }
    }
}
