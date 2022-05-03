using Client.Infrastructure.Managers.Interfaces;

namespace Client.Infrastructure.Managers
{
    public class BlockchainManager : ManagerBase, IBlockchainManager
    {
        public BlockchainManager(IManagerToolkit managerToolkit) : base(managerToolkit)
        {
        }

        public string Network => ManagerToolkit.AelfSettings.Network;
        public string Node => ManagerToolkit.AelfSettings.Node;
    }
}
