using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Domain.Coin;
using Gold.IO.Exchange.API.Storage.Interfaces;

namespace Gold.IO.Exchange.API.BusinessLogic.Services
{
    public class CoinService : BaseCrudService<Coin>, ICoinService
    {
        public CoinService(IRepository<Coin> repository) : base(repository)
        {
        }
    }
}
