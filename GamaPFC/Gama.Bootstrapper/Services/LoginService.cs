using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Bootstrapper.Services
{
    public class LoginService
    {
        public LoginService()
        {

        }

        public bool CheckCredentials(Modulos modulo, string user, string password)
        {
            bool result = false;

            switch (modulo)
            {
                case Modulos.ServicioDeAtenciones:
                    result = _CheckCredentials("GamaAtencionesMySql", user, password);
                    break;
                case Modulos.GestionDeSocios:
                    result = _CheckCredentials("GamaSociosMySql", user, password);
                    break;
                case Modulos.Cooperacion:
                    result = _CheckCredentials("GamaCooperacionMySql", user, password);
                    break;
            }

            return result;
        }

        private bool _CheckCredentials(string connectionName, string user, string password)
        {
            bool result = false;

            MySqlConnection connection =
                        new MySqlConnection(
                            ConfigurationManager.ConnectionStrings[connectionName].ConnectionString);

            string query = "SELECT * FROM usuarios WHERE nombre = @nombre AND password = @password";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = user;
            command.Parameters.Add("@password", MySqlDbType.VarChar).Value = password;

            try
            {
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                    result = true;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return result;
        }
    }
}
