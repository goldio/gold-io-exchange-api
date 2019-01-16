using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Gold.IO.Exchange.API.Storage.Mappings;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Gold.IO.Exchange.API.Storage
{
    public class NHibernateConfigurator
    {
        /// <summary>
        /// Интерфейс SessionFactory
        /// </summary>
        public interface ISessionFactory
        {
            /// <summary>
            /// Сессия
            /// </summary>
            ISession Session { get; }
        }

        /// <summary>
        /// The nhibernate configurator.
        /// </summary>
        public class NHibernateConfiguration : ISessionFactory
        {
            /// <summary>
            /// Фабрика сессий
            /// </summary>
            private readonly NHibernate.ISessionFactory _sessionFactory;

            /// <summary>
            /// Конструктор, инициализирует объект <see cref="NHibernateConfiguration"/> класса
            /// </summary>
            public NHibernateConfiguration(string connectionString)
            {
                var hibernateConfig = Fluently.Configure().Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserMap>())
                    .Database(MySQLConfiguration.Standard.ConnectionString(connectionString).ShowSql()).BuildConfiguration();

                var settings = GetSettings();
                if (!settings.TablesExist)
                {
                    var exporter = new SchemaExport(hibernateConfig);
                    exporter.Execute(true, true, false);

                    settings.TablesExist = true;
                    CreateOrUpdateSettings(settings);
                }

                _sessionFactory = hibernateConfig.BuildSessionFactory();
            }

            /// <summary>
            /// Получить сессию
            /// </summary>
            public ISession Session => _sessionFactory.OpenSession();

            private SettingsModel GetSettings()
            {
                var filename = "settings.json";
                var path = $"{System.IO.Directory.GetCurrentDirectory()}\\{filename}";

                return JsonConvert.DeserializeObject<SettingsModel>(System.IO.File.ReadAllText(path));
            }

            private void CreateOrUpdateSettings(SettingsModel model)
            {
                var filename = "settings.json";
                var path = $"{System.IO.Directory.GetCurrentDirectory()}\\{filename}";
                System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(model));
            }
        }

        class SettingsModel
        {
            public bool TablesExist { get; set; }
        }
    }
}
