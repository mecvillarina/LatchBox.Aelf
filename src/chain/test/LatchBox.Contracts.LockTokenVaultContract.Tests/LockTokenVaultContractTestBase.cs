using AElf.Boilerplate.TestBase;
using AElf.Cryptography.ECDSA;

namespace LatchBox.Contracts.LockTokenVaultContract
{
    public class LockTokenVaultContractTestBase : DAppContractTestBase<LockTokenVaultContractTestModule>
    {
        // You can get address of any contract via GetAddress method, for example:
        // internal Address DAppContractAddress => GetAddress(DAppSmartContractAddressNameProvider.StringName);

        internal LockTokenVaultContractContainer.LockTokenVaultContractStub GetLockTokenVaultContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<LockTokenVaultContractContainer.LockTokenVaultContractStub>(DAppContractAddress, senderKeyPair);
        }
    }
}