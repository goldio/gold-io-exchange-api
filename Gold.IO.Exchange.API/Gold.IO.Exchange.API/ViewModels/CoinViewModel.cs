using Gold.IO.Exchange.API.Domain;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class CoinViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public FileViewModel Icon { get; set; }
        public bool IsCrypto { get; set; }

        public CoinViewModel() { }

        public CoinViewModel(Coin coin)
        {
            ID = coin.ID;
            Name = coin.Name;
            ShortName = coin.ShortName;
            IsCrypto = coin.IsCrypto;

            Icon = (FileViewModel)coin.Icon;
        }
    }
}
