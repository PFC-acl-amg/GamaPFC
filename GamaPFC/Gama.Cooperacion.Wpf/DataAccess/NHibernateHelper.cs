using Core.DataAccess;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using Gama.Cooperacion.Wpf.Mappings;
using NHibernate;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;
using System.Configuration;

namespace Gama.Cooperacion.Wpf.DataAccess
{
    public class NHibernateHelper : INHibernateHelper
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
                        NHibernate.Cfg.Configuration configuration = Configure();

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

        private static NHibernate.Cfg.Configuration Configure()
        {
            return Fluently.Configure()
                .Database(MySQLConfiguration.Standard.ConnectionString(_connectionString))
                .Mappings(m => m.FluentMappings
                                .Add<ActividadMap>()
                                .Conventions.Add(DefaultCascade.All()))
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

            return SessionFactory.GetCurrentSession();
        }
    }
}
