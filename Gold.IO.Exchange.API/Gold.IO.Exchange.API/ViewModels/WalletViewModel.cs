using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Domain.Coin;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class WalletViewModel
    {
        public long ID { get; set; }
        public UserViewModel User { get; set; }
        public CoinViewModel Coin { get; set; }
        public double Balance { get; set; }
        public double OnOrders { get; set; }
        public double VTCValue { get; set; }

        public WalletViewModel() { }

        public WalletViewModel(Wallet wallet)
        {
            ID = wallet.ID;
            Coin = new CoinViewModel(wallet.Coin);
            User = new UserViewModel(wallet.User);
            Balance = wallet.Balance;
            OnOrders = 0;
            VTCValue = 0;
        }

        public WalletViewModel(Wallet wallet, double onOrders)
        {
            ID = wallet.ID;
            Coin = new CoinViewModel(wallet.Coin);
            User = new UserViewModel(wallet.User);
            Balance = wallet.Balance;
            OnOrders = onOrders;
            VTCValue = 0;
        }

        public WalletViewModel(Wallet wallet, double onOrders, double vtcValue)
        {
            ID = wallet.ID;
            Coin = new CoinViewModel(wallet.Coin);
            User = new UserViewModel(wallet.User);
            Balance = wallet.Balance;
            OnOrders = onOrders;
            VTCValue = vtcValue;
        }
    }
}
