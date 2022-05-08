using AElf.Types;
using Google.Protobuf.Collections;

namespace LatchBox.Contracts.MultiCrowdSaleContract
{
    public partial class MultiCrowdSaleContract
    {
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
            var crowdSaleIds = new RepeatedField<long>();

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
                crowdSaleIds = State.ActiveCrowdSales.Value.Ids;

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
    }
}
