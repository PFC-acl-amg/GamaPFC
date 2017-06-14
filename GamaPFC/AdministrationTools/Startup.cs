using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AdministrationTools.Startup))]
namespace AdministrationTools
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
