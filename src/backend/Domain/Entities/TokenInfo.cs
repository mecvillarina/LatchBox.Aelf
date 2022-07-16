namespace Domain.Entities
{
    public class TokenInfo
    {
        public int Id { get; set; }
        public int ChainId { get; set; }
        public string Symbol { get; set; }
        public string RawData { get; set; }
        public string Issuer { get; set; }
    }
}
