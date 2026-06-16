using Keystone.Application.Common.Interfaces;
using Keystone.Application.Common.Messaging;
using Keystone.Domain.Common;
using Keystone.Domain.Customers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Keystone.Infrastructure.Persistence;

/// <summary>
/// The EF Core context. It is also the unit of work, and it dispatches domain
/// events after a successful save (collected before save, cleared, then published
/// once the transaction has committed). See docs/adr/0003.
/// </summary>
public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IPublisher publisher) : DbContext(options), IUnitOfWork
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity)
            .Where(entity => entity.DomainEvents.Count > 0)
            .ToArray();

        var domainEvents = aggregates.SelectMany(aggregate => aggregate.DomainEvents).ToArray();
        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = (INotification)Activator.CreateInstance(notificationType, domainEvent)!;
            await publisher.Publish(notification, cancellationToken);
        }

        return result;
    }
}
