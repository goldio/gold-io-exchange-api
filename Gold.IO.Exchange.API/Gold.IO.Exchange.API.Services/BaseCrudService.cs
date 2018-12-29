using Gold.IO.Exchange.API.Services.Interfaces;
using Gold.IO.Exchange.API.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gold.IO.Exchange.API.Services
{
    public class BaseCrudService<T> : IBaseCrudService<T> where T : PersistentObject, new()
    {
        protected readonly IRepository<T> Repository;

        public BaseCrudService(IRepository<T> repository) => Repository = repository;

        public virtual IQueryable<T> GetAll()
        {
            return typeof(IDeletableObject).IsAssignableFrom(typeof(T))
                ? Repository.GetAll().Where(x => ((IDeletableObject)x).Deleted == false)
                : Repository.GetAll();
        }

        public virtual T Get(long id)
        {
            return typeof(IDeletableObject).IsAssignableFrom(typeof(T))
                ? Repository.GetAll().FirstOrDefault(x => x.ID == id && ((IDeletableObject)x).Deleted == false)
                : Repository.GetAll().FirstOrDefault(x => x.ID == id);
        }

        public virtual void Create(T item)
        {
            if (item == null)
                throw new ArgumentException("Object cannot be null, while creating object");

            if (item.ID != 0)
                throw new ArgumentException("You cannot create object with ID greater than 0");

            Repository.Add(item);
        }

        public virtual void Delete(long id)
        {
            if (id == 0)
                throw new ArgumentException("Не возможно удалить объект с ID равным 0");

            Delete(Get(id));
        }

        public virtual void Delete(T item)
        {
            if (item == null)
                throw new ArgumentException("Не возможно удалить Null объект");

            if (typeof(IDeletableObject).IsAssignableFrom(typeof(T)) == false)
            {
                Repository.Remove(item);
                return;
            }

            var deletableItem = (IDeletableObject)item;

            deletableItem.Deleted = true;
            item = (T)deletableItem;
            Update(item);
        }

        public virtual void Update(T item)
        {
            if ((item?.ID ?? 0) == 0)
            {
                throw new Exception("Для обновления объекта требуется идентификатор");
            }

            Repository.Update(item);
        }
    }
}
