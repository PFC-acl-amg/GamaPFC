
using Core.DataAccess;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using Gama.Cooperacion.DataAccess.Mappings;
using NHibernate;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gama.Cooperacion.DataAccess
{
    public class NHibernateSessionFactory : INHibernateSessionFactory
    {
        public static bool _EXECUTE_DDL = false;
        private static string _connectionString = ConfigurationManager.ConnectionStrings["GamaCooperacionMySql"].ConnectionString;

        private ISessionFactory _SessionFactory = null;
        public ISessionFactory SessionFactory
        {
            get
            {
                if (_SessionFactory == null)
                {
                    try
                    {
                        NHibernate.Cfg.Configuration configuration;

                        var directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                                + @"\GamaData\";

                        if (!Directory.Exists(directoryPath))
                            Directory.CreateDirectory(directoryPath);

                        var path = directoryPath + @"\nh_cooperacion.cfg";

                        if (File.Exists(path)) { File.Delete(path); }
                            
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

                        _SessionFactory = configuration.BuildSessionFactory();
                    }
                    catch (FluentConfigurationException ex)
                    {
                        throw new FluentConfigurationException($"Error: Session Factory\n - {ex.Message}", null);
                    }
                }

                return _SessionFactory;
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
                //.ShowSql())
                .Mappings(m => 
                    m.FluentMappings
                        .AddFromAssemblyOf<ActividadMap>()
                        .Conventions.Add(DefaultCascade.Delete(), DefaultLazy.Always()))
                .ExposeConfiguration(
                    c => {
                        var schema = new SchemaExport(c);
                        c.SetProperty("current_session_context_class", "thread_static");
                        schema.Execute(
                            useStdOut: false,
                            execute: _EXECUTE_DDL,// A true trunca las tablas cada vez ejecutas el programa
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
            var session = SessionFactory.OpenSession();
            session.FlushMode = FlushMode.Commit;
            return session;
        }
    }
}
