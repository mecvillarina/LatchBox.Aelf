namespace Client.Infrastructure.Managers.Interfaces
{
    public interface IBlockchainManager : IManager
    {
        string Network { get; }
        string Node { get; }
    }
}