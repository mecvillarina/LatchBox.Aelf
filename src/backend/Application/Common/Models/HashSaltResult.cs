namespace Application.Common.Models
{
    public class HashSaltResult
    {
        public HashSaltResult(string salt, string digest)
        {
            Salt = salt;
            Digest = digest;
        }

        public string Salt { get; }
        public string Digest { get; set; }
    }
}