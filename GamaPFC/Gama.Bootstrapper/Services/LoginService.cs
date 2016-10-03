using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Bootstrapper.Services
{
    public class LoginService : ILoginService
    {
        public bool CheckCredentials(string user, string password)
        {
            bool result = false;

            switch (user)
            {
                case "atenciones":
                    result = password == "secret";
                    break;
                case "socios":
                    result = password == "secret";
                    break;
                case "cooperacion":
                    result = password == "secret";
                    break;
                default:
                    throw new Exception("¡El usuario no existe!");
            }

            return result;
        }
    }
}
