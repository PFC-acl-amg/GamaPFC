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
        public static bool _EXECUTE_DDL = false;

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

                        var directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                                + @"\GamaData\";

                        if (!Directory.Exists(directoryPath))
                            Directory.CreateDirectory(directoryPath);

                        var path = directoryPath + @"\nh_socios.cfg";

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
                        .AddFromAssemblyOf <SocioMap> ()
                        //.Add<ActividadMap>()
                        //.Add<CooperanteMap>()
                        .Conventions.Add(DefaultCascade.All(), DefaultLazy.Never()))
                .ExposeConfiguration(
                    c => {
                        var schema = new SchemaExport(c);
                        c.SetProperty("current_session_context_class", "thread_static");
                        schema.Execute(
                            useStdOut: false,
                            execute: _EXECUTE_DDL, 
                            justDrop: false);
                    })
                .BuildConfiguration();
        }
        //_EXECUTE_DDL

        public ISession OpenSession()
        {
            var session = SessionFactory.OpenSession(); // Se cargar la base de datos siempre que se ejecutar
            session.FlushMode = FlushMode.Commit;
            return session;
        }
    }
}
