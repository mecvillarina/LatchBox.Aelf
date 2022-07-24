using System.Collections.Generic;

namespace Client.App.SmartContractDto.LockTokenVault
{
    public class LockListOutput
    {
        public List<LockOutput> Locks { get; set; } = new();
    }
}
