using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PomfSharp.Startup))]
namespace PomfSharp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
