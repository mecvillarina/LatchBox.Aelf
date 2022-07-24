using System.Collections.Generic;

namespace Client.App.SmartContractDto.Launchpad
{
    public class CrowdSaleByInvestorListOutput
    {
        public List<CrowdSaleInvestmentOutput> List { get; set; } = new();
    }
}
