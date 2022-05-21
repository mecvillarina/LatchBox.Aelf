namespace Client.Infrastructure.Models
{
    public class WalletAuthHandler
    {
        public string Address { get; set; }
        public AuthTokenHandler TokenHandler { get; set; }
    }
}
