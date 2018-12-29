using NHibernate;

namespace Storage.Interfaces
{
    public interface ISessionStorage
    {
        ISession Session { get; }
    }
}
