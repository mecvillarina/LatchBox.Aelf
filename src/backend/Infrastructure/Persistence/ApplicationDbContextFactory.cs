using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private readonly ICallContext _context;
        private readonly IDateTime _dateTime;
        private readonly IDomainEventService _domainEventService;
        private readonly IConfiguration _configuration;
        public ApplicationDbContextFactory(IDomainEventService domainEventService, ICallContext context, IDateTime dateTime, IConfiguration configuration)
        {
            _domainEventService = domainEventService;
            _context = context;
            _dateTime = dateTime;
            _configuration = configuration;
        }

        public ApplicationDbContextFactory()
        {
        }

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../WebApi"))
                    .AddJsonFile("local.settings.json", false, true)
                    .AddEnvironmentVariables()
                    .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = config.GetConnectionString("DataDbContext");
            optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            return new ApplicationDbContext(optionsBuilder.Options, _domainEventService, _dateTime);
        }
    }
}