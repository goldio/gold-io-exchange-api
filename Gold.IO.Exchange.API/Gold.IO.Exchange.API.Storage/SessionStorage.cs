using NHibernate;
using Storage.Interfaces;
using System;

namespace Gold.IO.Exchange.API.Storage
{
    /// <summary>
    /// Хранилище для сессий. Создан потому, что в альфа версии
    /// DI нет возможности зарегистрировать интерфейс с помощью фабрики
    /// ISessionStorage регистрируется "per web request" и всегда возвращает один и тот же инстанс сессии
    /// </summary>
    public class SessionStorage : ISessionStorage
    {
        private readonly NHibernateConfigurator.ISessionFactory _sessionFactory;

        private ISession _session;

        public SessionStorage(NHibernateConfigurator.ISessionFactory sf)
        {
            _sessionFactory = sf;
        }

        public ISession Session
        {
            get
            {
                if (_session != null) return _session;

                lock (this)
                {
                    _session = _session ?? _sessionFactory.Session;
                }

                return _session;
            }

            private set
            {
                throw new NotImplementedException();
            }
        }
    }
}
