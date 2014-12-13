using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MNIT_Communication.Startup))]
namespace MNIT_Communication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
