using Application.Common.Dtos;

namespace Application.Common.Interfaces
{
    public interface IAElfService
    {
        ChainStatusDto GetChainStatus(string apiUrl);
    }
}
