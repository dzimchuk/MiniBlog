using LightInject;
using MiniBlog.Contracts;

namespace MiniBlog.Azure.Composition
{
    public class CompositionModule : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IFileStorage, FileStorage>("AzureBlobStorage");
            serviceRegistry.Register<IFileStorage>(factory =>
                new CdnFileStorage(factory.GetInstance<IFileStorage>("AzureBlobStorage"), factory.GetInstance<IConfiguration>()));
            serviceRegistry.Register<IPostStorage, PostStorage>();
        }
    }
}