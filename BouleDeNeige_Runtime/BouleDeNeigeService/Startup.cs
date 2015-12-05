using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BouleDeNeigeService.Startup))]

namespace BouleDeNeigeService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}