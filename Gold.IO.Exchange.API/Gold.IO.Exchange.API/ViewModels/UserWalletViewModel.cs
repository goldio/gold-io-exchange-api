
using Gold.IO.Exchange.API.Domain.User;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class UserWalletViewModel
    {
        public long ID { get; set; }
        public UserViewModel User { get; set; }
        public CoinViewModel Coin { get; set; }
        public double TotalBalance { get; set; }
        public double AvailableBalance { get; set; }
        public double InOrders { get; set; }
        public double Cost { get; set; }

        public UserWalletViewModel() { }

        public UserWalletViewModel(UserWallet wallet)
        {
            ID = wallet.ID;
            Coin = new CoinViewModel(wallet.Coin);
            User = new UserViewModel(wallet.User);
            TotalBalance = wallet.Balance;
            AvailableBalance = wallet.Balance;
            InOrders = 0;
            Cost = 0;
        }

        public UserWalletViewModel(UserWallet wallet, double inOrders)
        {
            ID = wallet.ID;
            Coin = new CoinViewModel(wallet.Coin);
            User = new UserViewModel(wallet.User);
            TotalBalance = wallet.Balance;
            AvailableBalance = wallet.Balance;
            InOrders = inOrders;
            Cost = 0;
        }

        public UserWalletViewModel(UserWallet wallet, double inOrders, double cost)
        {
            ID = wallet.ID;
            Coin = new CoinViewModel(wallet.Coin);
            User = new UserViewModel(wallet.User);
            TotalBalance = wallet.Balance;
            AvailableBalance = wallet.Balance;
            InOrders = inOrders;
            Cost = cost;
        }
    }
}
