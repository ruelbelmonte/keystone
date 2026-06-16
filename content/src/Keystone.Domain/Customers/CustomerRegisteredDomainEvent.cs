using Keystone.Domain.Common;

namespace Keystone.Domain.Customers;

public sealed record CustomerRegisteredDomainEvent(CustomerId CustomerId) : IDomainEvent;
