
using Gold.IO.Exchange.API.Domain.User;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class UserWalletViewModel
    {
        public long ID { get; set; }
        public UserViewModel User { get; set; }
        public CoinViewModel Coin { get; set; }
        public WalletAddressViewModel Address { get; set; }
        public double Balance { get; set; }
        public double OnOrders { get; set; }
        public double VTCValue { get; set; }

        public UserWalletViewModel() { }

        public UserWalletViewModel(UserWallet wallet)
        {
            ID = wallet.ID;
            Coin = new CoinViewModel(wallet.Coin);
            User = new UserViewModel(wallet.User);
            Address = new WalletAddressViewModel(wallet.Address);
            Balance = wallet.Balance;
            OnOrders = 0;
            VTCValue = 0;
        }

        public UserWalletViewModel(UserWallet wallet, double onOrders)
        {
            ID = wallet.ID;
            Coin = new CoinViewModel(wallet.Coin);
            User = new UserViewModel(wallet.User);
            Address = new WalletAddressViewModel(wallet.Address);
            Balance = wallet.Balance;
            OnOrders = onOrders;
            VTCValue = 0;
        }

        public UserWalletViewModel(UserWallet wallet, double onOrders, double vtcValue)
        {
            ID = wallet.ID;
            Coin = new CoinViewModel(wallet.Coin);
            User = new UserViewModel(wallet.User);
            Address = new WalletAddressViewModel(wallet.Address);
            Balance = wallet.Balance;
            OnOrders = onOrders;
            VTCValue = vtcValue;
        }
    }
}
