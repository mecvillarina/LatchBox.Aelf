﻿using AElf.Client.LatchBox.MultiCrowdSale;
using MudBlazor;

namespace Client.Infrastructure.Models
{
    public class MyCrowdSaleModel
    {
        public CrowdSale CrowdSale { get; }
        public long RaisedAmount { get; }
        public string TokenName { get; }
        public string TokenSymbol { get; }
        public int TokenDecimals { get; }
        public string Status { get; set; }
        public Color StatusColor { get; set; }
        public bool CanCancel { get; set; }
        public bool CanComplete { get; set; }
        public bool CanRefund { get; set; }

        public MyCrowdSaleModel(CrowdSaleOutput output)
        {
            CrowdSale = output.CrowdSale;
            RaisedAmount = output.RaisedAmount;
            TokenName = output.TokenName;
            TokenSymbol = output.TokenSymbol;
            TokenDecimals = output.TokenDecimals;

            if (CrowdSale.IsActive)
            {
                CanCancel = true;

                if(CrowdSale.SaleStartDate.ToDateTimeOffset() > System.DateTimeOffset.UtcNow)
                {
                    Status = "UPCOMING";
                    StatusColor = Color.Primary;
                }
                if (CrowdSale.SaleEndDate.ToDateTimeOffset() > System.DateTimeOffset.UtcNow)
                {
                    Status = "ON SALE";
                    StatusColor = Color.Primary;
                }
                else
                {
                    Status = "ENDED";
                    StatusColor = Color.Info;
                    CanComplete = true;
                }
            }
            else
            {
                if (!CrowdSale.IsCancelled)
                {
                    if (CrowdSale.HardCapNativeTokenAmount == RaisedAmount || RaisedAmount >= CrowdSale.SoftCapNativeTokenAmount)
                    {
                        Status = "SUCCESS";
                        StatusColor = Color.Info;
                        
                    }
                    else
                    {
                        CanRefund = true;
                        Status = "GOAL NOT MET";
                        StatusColor = Color.Error;
                    }
                }
                else
                {
                    CanRefund = true;
                    Status = "CANCELLED";
                    StatusColor = Color.Error;
                }
            }
        }
    }
}