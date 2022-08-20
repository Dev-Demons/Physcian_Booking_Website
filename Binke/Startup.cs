using Microsoft.Owin;
using Owin;
[assembly: OwinStartupAttribute(typeof(Binke.Startup))]
namespace Binke
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AutoMapperConfigure();
            ConfigureAuth(app);
        }
    }

}
