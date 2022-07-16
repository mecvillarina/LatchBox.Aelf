namespace Application.Common.Dtos
{
    public class TokenBalanceInfoDto
    {
        public TokenDto Token { get; set; }
        public string Balance { get; set; }
        public bool IsIssuer { get; set; }
    }
}
