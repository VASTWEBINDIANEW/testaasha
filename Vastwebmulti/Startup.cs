using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Vastwebmulti.Startup))]
namespace Vastwebmulti
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
