using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<ChainInfo> ChainInfos { get; set; }
        DbSet<TokenInfo> TokenInfos { get; set; }
        DbSet<CrossChainOperation> CrossChainOperations { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
        int SaveChanges();
    }
}