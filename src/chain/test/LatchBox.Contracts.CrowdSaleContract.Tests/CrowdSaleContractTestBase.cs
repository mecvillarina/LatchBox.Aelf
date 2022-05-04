using AElf.Boilerplate.TestBase;
using AElf.Cryptography.ECDSA;

namespace LatchBox.Contracts.CrowdSaleContract
{
    public class CrowdSaleContractTestBase : DAppContractTestBase<CrowdSaleContractTestModule>
    {
        // You can get address of any contract via GetAddress method, for example:
        // internal Address DAppContractAddress => GetAddress(DAppSmartContractAddressNameProvider.StringName);

        internal CrowdSaleContractContainer.CrowdSaleContractStub GetCrowdSaleContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<CrowdSaleContractContainer.CrowdSaleContractStub>(DAppContractAddress, senderKeyPair);
        }
    }
}