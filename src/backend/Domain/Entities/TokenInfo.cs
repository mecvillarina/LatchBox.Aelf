namespace Domain.Entities
{
    public class TokenInfo
    {
        public int Id { get; set; }
        public int ChainId { get; set; }
        public string Symbol { get; set; }
        public string RawExplorerData { get; set; }
        public string RawTxData { get; set; }
        public string Issuer { get; set; }
    }
}
