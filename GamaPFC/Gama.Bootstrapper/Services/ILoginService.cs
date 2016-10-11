namespace Gama.Bootstrapper.Services
{
    public interface ILoginService
    {
        bool CheckCredentials(string user, string password);
    }
}