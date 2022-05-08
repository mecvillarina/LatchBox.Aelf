using AElf.Types;

namespace LatchBox.Contracts.MultiCrowdSaleContract
{
    public partial class MultiCrowdSaleContract
    {
        public override CrowdSaleOutput GetCrowdSale(GetCrowdSaleInput input)
        {
            Assert(input.CrowdSaleId < State.SelfIncresingCrowdSaleId.Value, "Invalid Sale Id");

            return GetCrowdSaleOutput(input.CrowdSaleId);
        }

        public override CrowdSaleListOutput GetCrowdSalesByInitiator(Address input)
        {
            var crowdSalesByInitiator = State.CrowdSalesByInitiator[input] ?? new CrowdSaleIds();

            var output = new CrowdSaleListOutput();

            foreach (var crowdSaleId in crowdSalesByInitiator.Ids)
            {
                output.CrowdSales.Add(GetCrowdSaleOutput(crowdSaleId));
            }

            return output;
        }

        public override CrowdSaleListOutput GetCrowdSales(GetCrowdSalesInput input)
        {
            var output = new CrowdSaleListOutput();

            if (!input.IsUpcoming && !input.IsOngoing)
            {
                for (int i = 1; i < State.SelfIncresingCrowdSaleId.Value; i++)
                {
                    var sale = GetCrowdSaleOutput(i);
                    output.CrowdSales.Add(sale);
                }
            }
            else
            {
                var crowdSaleIds = State.ActiveCrowdSales.Value.Ids;

                foreach (var crowdSaleId in crowdSaleIds)
                {
                    var sale = GetCrowdSaleOutput(crowdSaleId);
                    if (input.IsUpcoming && sale.CrowdSale.SaleStartDate >= Context.CurrentBlockTime)
                    {
                        output.CrowdSales.Add(sale);
                    }
                    else if (input.IsOngoing && sale.CrowdSale.SaleStartDate < Context.CurrentBlockTime && sale.CrowdSale.SaleEndDate > Context.CurrentBlockTime)
                    {
                        output.CrowdSales.Add(sale);
                    }
                }
            }

            return output;
        }

        public override CrowdSaleInvestorListOutput GetCrowdSaleInvestors(GetCrowdSaleInvestorsInput input)
        {
            Assert(input.CrowdSaleId < State.SelfIncresingCrowdSaleId.Value, "Invalid Sale Id");

            var crowdSaleId = input.CrowdSaleId;
            var output = new CrowdSaleInvestorListOutput();

            var investors = State.CrowdSaleInvestors[input.CrowdSaleId].Investors;

            foreach (var investor in investors)
            {
                var purchase = State.CrowdSalePurchases[crowdSaleId][investor];
                output.Purchases.Add(purchase);
            }

            return output;
        }
    }
}
