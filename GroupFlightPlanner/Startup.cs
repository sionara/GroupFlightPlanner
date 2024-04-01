using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GroupFlightPlanner.Startup))]
namespace GroupFlightPlanner
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
