namespace Client.App.Models
{
    public class NightElfModel
    {
        public bool HasExtension { get; set; }
        public bool IsConnected { get; set; }
        public string WalletAddress { get; set; }

        public void Clear()
        {
            IsConnected = false;
            WalletAddress = null;
        }
    }
}
