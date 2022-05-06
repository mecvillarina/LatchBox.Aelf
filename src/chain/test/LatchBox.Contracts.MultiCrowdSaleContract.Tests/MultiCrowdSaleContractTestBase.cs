using AElf.Boilerplate.TestBase;
using AElf.Cryptography.ECDSA;

namespace LatchBox.Contracts.MultiCrowdSaleContract
{
    public class MultiCrowdSaleContractTestBase : DAppContractTestBase<MultiCrowdSaleContractTestModule>
    {
        // You can get address of any contract via GetAddress method, for example:
        // internal Address DAppContractAddress => GetAddress(DAppSmartContractAddressNameProvider.StringName);

        internal MultiCrowdSaleContractContainer.MultiCrowdSaleContractStub GetMultiCrowdSaleContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<MultiCrowdSaleContractContainer.MultiCrowdSaleContractStub>(DAppContractAddress, senderKeyPair);
        }
    }
}