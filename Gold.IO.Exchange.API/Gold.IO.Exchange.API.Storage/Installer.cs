using Gold.IO.Exchange.API.Storage.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Storage.Interfaces;
using Storage.Repositories;

namespace Gold.IO.Exchange.API.Storage
{
    public static class Installer
    {
        public static void AddNHibernate(this IServiceCollection container, string connectionString)
        {
            container.AddSingleton(typeof(NHibernateConfigurator.ISessionFactory), new NHibernateConfigurator.NHibernateConfiguration(connectionString));
            container.AddScoped<ISessionStorage, SessionStorage>();
            container.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}
