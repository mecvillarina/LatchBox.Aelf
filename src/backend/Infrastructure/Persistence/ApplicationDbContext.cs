using Application.Common.Interfaces;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly IDateTime _dateTime;
        private readonly IDomainEventService _domainEventService;

        public ApplicationDbContext(DbContextOptions options, IDomainEventService domainEventService, IDateTime dateTime) : base(options)
        {
            _domainEventService = domainEventService;
            _dateTime = dateTime;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                var utcNow = _dateTime.UtcNow;

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = utcNow;
                        entry.Entity.UpdatedAt = utcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = utcNow;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);
            await DispatchEvents();
            return result;
        }

        public override int SaveChanges()
        {
            var result = base.SaveChanges();
            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        private async Task DispatchEvents()
        {
            while (true)
            {
                var domainEventEntity = ChangeTracker.Entries<IHasDomainEvent>()
                        .Select(x => x.Entity.DomainEvents)
                        .SelectMany(x => x)
                        .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

                if (domainEventEntity == null) break;

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity);
            }
        }
    }
}