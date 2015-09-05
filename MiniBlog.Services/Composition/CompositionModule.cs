using LightInject;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Framework;

namespace MiniBlog.Services.Composition
{
    public class CompositionModule : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IConfiguration, Configuration>();
            serviceRegistry.Register<ILocalPathProvider, LocalPathProvider>();
            serviceRegistry.Register<IPostSerializer, PostSerializer>();
            serviceRegistry.Register<IAuthenticationService, AuthenticationService>();
        }
    }
}