using AElf.Types;
using MediatR;

namespace Client.App.SmartContractDto
{
    public class TokenGetAllowanceInput
    {
        public string Symbol { get; set; }
        public string Owner { get; set; }
        public string Spender { get; set; }
    }
}
