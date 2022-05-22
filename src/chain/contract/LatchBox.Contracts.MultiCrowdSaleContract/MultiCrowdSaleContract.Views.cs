using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace LatchBox.Contracts.MultiCrowdSaleContract
{
    public partial class MultiCrowdSaleContract
    {
        public override Int64Value GetCrowdSaleCount(Empty input)
        {
            return new Int64Value { Value = State.SelfIncresingCrowdSaleId.Value - 1 };
        }

        public override Int64Value GetUpcomingCrowdSaleCount(Empty input)
        {
            var crowdSaleIds = State.ActiveCrowdSales.Value.Ids;
            long count = 0;
            foreach (var crowdSaleId in crowdSaleIds)
            {
                var sale = GetCrowdSaleOutput(crowdSaleId);
                if (sale.CrowdSale.SaleStartDate >= Context.CurrentBlockTime)
                {
                    count++;
                }
            }

            return new Int64Value { Value = count };
        }

        public override Int64Value GetOngoingCrowdSaleCount(Empty input)
        {
            var crowdSaleIds = State.ActiveCrowdSales.Value.Ids;
            long count = 0;

            foreach (var crowdSaleId in crowdSaleIds)
            {
                var sale = GetCrowdSaleOutput(crowdSaleId);
                if (sale.CrowdSale.SaleStartDate < Context.CurrentBlockTime && sale.CrowdSale.SaleEndDate > Context.CurrentBlockTime)
                {
                    count++;
                }
            }

            return new Int64Value { Value = count };
        }


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
                output.List.Add(GetCrowdSaleOutput(crowdSaleId));
            }

            return output;
        }

        public override CrowdSaleByInvestorListOuput GetCrowdSalesByInvestor(Address input)
        {
            var crowdSalesByInvestor = State.CrowdSalesByInvestor[input] ?? new CrowdSaleIds();

            var output = new CrowdSaleByInvestorListOuput();

            foreach (var crowdSaleId in crowdSalesByInvestor.Ids)
            {
                var crowdSaleOutput = GetCrowdSaleOutput(crowdSaleId);

                output.List.Add(new CrowdSaleInvestmentOutput()
                {
                    CrowdSaleOutput = crowdSaleOutput,
                    Investment = State.CrowdSaleInvestments[crowdSaleId][input]
                });
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
                    output.List.Add(sale);
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
                        output.List.Add(sale);
                    }
                    else if (input.IsOngoing && sale.CrowdSale.SaleStartDate < Context.CurrentBlockTime && sale.CrowdSale.SaleEndDate > Context.CurrentBlockTime)
                    {
                        output.List.Add(sale);
                    }
                }
            }

            return output;
        }

        public override CrowdSaleInvestmentListOutput GetCrowdSaleInvestments(GetCrowdSaleInvestorsInput input)
        {
            Assert(input.CrowdSaleId < State.SelfIncresingCrowdSaleId.Value, "Invalid Sale Id");

            var crowdSaleId = input.CrowdSaleId;
            var output = new CrowdSaleInvestmentListOutput();

            var investors = State.CrowdSaleInvestors[input.CrowdSaleId].Investors;

            foreach (var investor in investors)
            {
                var investment = State.CrowdSaleInvestments[crowdSaleId][investor];
                output.List.Add(investment);
            }

            return output;
        }
    }
}
