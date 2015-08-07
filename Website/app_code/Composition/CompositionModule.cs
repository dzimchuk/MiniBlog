using LightInject;
using MiniBlog.Contracts;

public class CompositionModule : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.Register<IFileStorage, LocalFileStorage>();
        serviceRegistry.Register<IConfiguration, Configuration>();
    }
}