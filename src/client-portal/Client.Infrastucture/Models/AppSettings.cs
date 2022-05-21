namespace Client.Infrastructure.Models
{
    public class AppSettings
    {
        public string Name { get; set; }
        public string JwtAudience { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtSecret { get; set; }
    }
}
