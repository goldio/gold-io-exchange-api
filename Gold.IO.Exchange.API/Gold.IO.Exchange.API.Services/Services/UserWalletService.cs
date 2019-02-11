using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.User;
using Gold.IO.Exchange.API.Storage.Interfaces;

namespace Gold.IO.Exchange.API.BusinessLogic.Services
{
    public class UserWalletService : BaseCrudService<UserWallet>, IUserWalletService
    {
        public UserWalletService(IRepository<UserWallet> repository) : base(repository)
        {
        }
    }
}
