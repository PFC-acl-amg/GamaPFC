using Core.DataAccess;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using Gama.Socios.DataAccess.Mappings;
using NHibernate;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.DataAccess
{
    public class NHibernateSessionFactory : INHibernateSessionFactory
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["GamaSociosMySql"].ConnectionString;

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

                        //File.Delete("nh_socios.cfg");
                        if (File.Exists("nh_socios.cfg"))
                        {
                            var file = File.Open("nh_socios.cfg", FileMode.Open);
                            configuration = (NHibernate.Cfg.Configuration)new BinaryFormatter()
                                .Deserialize(file);
                            file.Close();
                        }
                        else
                        {
                            configuration = Configure();
                            new BinaryFormatter().Serialize(File.Create("nh_socios.cfg"), configuration);
                        }

                        try
                        {
                            _sessionFactory = configuration.BuildSessionFactory();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    catch (FluentConfigurationException ex)
                    {
                        throw new FluentConfigurationException($"Error: Session Factory\n - {ex.Message}", ex);
                    }
                }

                return _sessionFactory;
            }
        }

        private static NHibernate.Cfg.Configuration Configure()
        {
            return Fluently.Configure()
                .Database(MySQLConfiguration.Standard.ConnectionString(_connectionString))
                //.ShowSql())
                .Mappings(m =>
                    m.FluentMappings
                        .AddFromAssemblyOf<SocioMap>()
                        //.Add<ActividadMap>()
                        //.Add<CooperanteMap>()
                        .Conventions.Add(DefaultCascade.All(), DefaultLazy.Always()))
                .ExposeConfiguration(
                    c => {
                        var schema = new SchemaExport(c);
                        c.SetProperty("current_session_context_class", "thread_static");
                        schema.Execute(
                            useStdOut: true,
                            execute: true,
                            justDrop: false);
                    })
                .BuildConfiguration();
        }

        public ISession OpenSession()
        {
            var session = SessionFactory.OpenSession();
            session.FlushMode = FlushMode.Commit;
            return session;
        }
    }
}
