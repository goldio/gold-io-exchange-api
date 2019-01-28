using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Storage.Interfaces;

namespace Gold.IO.Exchange.API.BusinessLogic.Services
{
    public class PersonService : BaseCrudService<Person>, IPersonService
    {
        public PersonService(IRepository<Person> repository) : base(repository)
        {
        }
    }
}
