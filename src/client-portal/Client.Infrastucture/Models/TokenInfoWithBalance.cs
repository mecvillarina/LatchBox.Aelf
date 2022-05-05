using AElf.Client.MultiToken;

namespace Client.Infrastructure.Models
{
    public class TokenInfoWithBalance
    {
        public bool IsNative { get; set; }
        public TokenInfo Token { get; set; }
        public long? Balance { get; set; }
    }
}
