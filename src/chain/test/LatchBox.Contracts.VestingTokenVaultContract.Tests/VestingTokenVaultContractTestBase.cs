using AElf.Boilerplate.TestBase;
using AElf.Cryptography.ECDSA;

namespace LatchBox.Contracts.VestingTokenVaultContract
{
    public class VestingTokenVaultContractTestBase : DAppContractTestBase<VestingTokenVaultContractTestModule>
    {
        // You can get address of any contract via GetAddress method, for example:
        // internal Address DAppContractAddress => GetAddress(DAppSmartContractAddressNameProvider.StringName);

        internal VestingTokenVaultContractContainer.VestingTokenVaultContractStub GetVestingTokenVaultContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<VestingTokenVaultContractContainer.VestingTokenVaultContractStub>(DAppContractAddress, senderKeyPair);
        }
    }
}