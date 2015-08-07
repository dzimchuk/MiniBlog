using LightInject;
using MiniBlog.Contracts;

namespace MiniBlog.Azure.Composition
{
    public class CompositionModule : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IFileStorage, BlobStorage>("AzureBlobStorage");
            serviceRegistry.Register<IFileStorage>(factory =>
                new CdnFileStorage(factory.GetInstance<IFileStorage>("AzureBlobStorage"), factory.GetInstance<IConfiguration>()));
        }
    }
}