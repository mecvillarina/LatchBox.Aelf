using Client.App.Parameters;

namespace Client.App.SmartContractDto
{
    public class IssueTokenInput
    {
        public string Symbol { get; set; }
        public long Amount { get; set; }
        public string Memo { get; set; }
        public string To { get; set; }

        public IssueTokenInput(IssueTokenParameter parameter)
        {
            Symbol = parameter.Symbol;
            Memo = parameter.Memo;
            To = parameter.To;
        }
    }
}
