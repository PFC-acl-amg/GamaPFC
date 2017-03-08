using Core.DataAccess;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using Gama.Atenciones.DataAccess.Mappings;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gama.Atenciones.DataAccess
{
    public class NHibernateSessionFactory : INHibernateSessionFactory
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["GamaAtencionesMySql"].ConnectionString;

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

                        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\nh_atenciones.cfg";
                       //File.Delete(path);
                        if (File.Exists(path))
                        {
                            var file = File.Open(path, FileMode.Open);
                            configuration = (NHibernate.Cfg.Configuration)new BinaryFormatter()
                                .Deserialize(file);
                            file.Close();
                        }
                        else
                        {
                            configuration = Configure();
                            new BinaryFormatter().Serialize(File.Create(path), configuration);
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
                        .AddFromAssemblyOf<PersonaMap>()
                        //.Add<ActividadMap>()
                        //.Add<CooperanteMap>()
                        .Conventions.Add(DefaultCascade.All(), DefaultLazy.Always()))
                .ExposeConfiguration(
                    c => {
                        var schema = new SchemaExport(c);
                        c.SetProperty("current_session_context_class", "thread_static");
                        schema.Execute(
                            useStdOut: false,
                            execute: false,
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
