using System.Collections.Generic;

namespace Client.App.Parameters
{
    public class AddVestingParameter
    {
        public string TokenSymbol { get; set; }
        public bool IsRevocable { get; set; }
        public List<UpsetVestingPeriodParameter> Periods { get; set; } = new List<UpsetVestingPeriodParameter>();
    }

}
