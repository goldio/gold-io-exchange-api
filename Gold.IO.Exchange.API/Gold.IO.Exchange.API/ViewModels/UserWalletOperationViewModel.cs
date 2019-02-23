using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class UserWalletOperationViewModel
    {
        public long ID { get; set; }
        public DateTime Time { get; set; }
        public CoinAddressViewModel Address { get; set; }
        public double Amount { get; set; }
        public long Confirmations { get; set; }
        public UserWalletOperationStatus Status { get; set; }

        public UserWalletOperationViewModel() { }

        public UserWalletOperationViewModel(UserWalletOperation userWalletOperation)
        {
            ID = userWalletOperation.ID;
            Time = userWalletOperation.Time;
            Address = new CoinAddressViewModel(userWalletOperation.Address);
            Amount = userWalletOperation.Amount;
            Confirmations = userWalletOperation.Confirmations;
            Status = userWalletOperation.Status;
        }
    }
}
