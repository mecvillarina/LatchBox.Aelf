using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
        int SaveChanges();
    }
}