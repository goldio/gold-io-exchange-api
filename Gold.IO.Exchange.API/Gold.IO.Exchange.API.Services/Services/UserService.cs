using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Domain.User;
using Gold.IO.Exchange.API.Storage.Interfaces;

namespace Gold.IO.Exchange.API.BusinessLogic.Services
{
    public class UserService : BaseCrudService<User>, IUserService
    {
        public UserService(IRepository<User> repository) : base(repository)
        {
        }
    }
}
