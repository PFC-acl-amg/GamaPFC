﻿using Core.DataAccess;
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
        public static bool _EXECUTE_DDL = false;
        private static string _connectionString = ConfigurationManager.ConnectionStrings["GamaAtencionesMySql"].ConnectionString;

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

                        var path = directoryPath + @"\nh_atenciones.cfg";

                       // if (File.Exists(path)) { File.Delete(path); }

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
                        throw new FluentConfigurationException($"Error: Session Factory\n - {ex.Message}", ex);
                    }
                }

                return _SessionFactory;
            }
        }

        private static NHibernate.Cfg.Configuration Configure()
        {
            return Fluently.Configure()
                .Database(MySQLConfiguration.Standard.ConnectionString(_connectionString))
                .Mappings(m =>
                    m.FluentMappings
                        .AddFromAssemblyOf<PersonaMap>()
                        .Conventions.Add(DefaultCascade.All(), DefaultLazy.Always()))
                .ExposeConfiguration(
                    c => {
                        var schema = new SchemaExport(c);
                        //c.SetProperty("current_session_context_class", "thread_static");
                        schema.Execute(
                            useStdOut: false,
                            execute: _EXECUTE_DDL,
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
