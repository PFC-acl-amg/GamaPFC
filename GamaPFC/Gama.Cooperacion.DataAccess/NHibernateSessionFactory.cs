
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using Gama.Cooperacion.DataAccess.Mappings;
using NHibernate;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gama.Cooperacion.DataAccess
{
    public class NHibernateSessionFactory : INHibernateSessionFactory
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["GamaMySql"].ConnectionString;

        private ISessionFactory _sessionFactory = null;
        public ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    try
                    {
                        NHibernate.Cfg.Configuration configuration;

                        if (File.Exists("nh.cfg"))
                        {
                            var file = File.Open("nh.cfg", FileMode.Open);
                            configuration = (NHibernate.Cfg.Configuration)new BinaryFormatter()
                                .Deserialize(file);
                            //file.Close();
                        }
                        else
                        {
                            configuration = Configure();
                            new BinaryFormatter().Serialize(File.Create("nh.cfg"), configuration);
                        }

                        _sessionFactory = configuration.BuildSessionFactory();
                    }
                    catch (FluentConfigurationException ex)
                    {
                        throw new FluentConfigurationException($"Error: Session Factory\n - {ex.Message}", null);
                    }
                }

                return _sessionFactory;
            }
        }

        public ISessionFactory GetSessionFactory()
        {
            return SessionFactory;
        }

        private static NHibernate.Cfg.Configuration Configure()
        {
            return Fluently.Configure()
                .Database(MySQLConfiguration.Standard.ConnectionString(_connectionString))
                .Mappings(m => m.FluentMappings
                                .Add<ActividadMap>()
                                .Add<CooperanteMap>()
                                .Conventions.Add(DefaultCascade.All(), DefaultLazy.Never()))
                .ExposeConfiguration(
                    c => {
                        var schema = new SchemaExport(c);
                        c.SetProperty("current_session_context_class", "thread_static");
                        schema.Execute(
                            useStdOut: true,
                            execute: false,
                            justDrop: false);
                    })
                .BuildConfiguration();
        }

        public IStatelessSession OpenStatelessSession()
        {
            return SessionFactory.OpenStatelessSession();
        }

        public ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        public void CreateSession()
        {
            CurrentSessionContext.Bind(OpenSession());
        }

        public void CloseSession()
        {
            if (CurrentSessionContext.HasBind(SessionFactory))
            {
                CurrentSessionContext.Unbind(SessionFactory).Dispose();
            }
        }

        public ISession GetCurrentSession()
        {
            if (!CurrentSessionContext.HasBind(SessionFactory))
            {
                CurrentSessionContext.Bind(SessionFactory.OpenSession());
            }

            //return SessionFactory.OpenSession();
            return SessionFactory.GetCurrentSession();
        }
    }
}
