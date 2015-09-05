using LightInject;
using MiniBlog.Azure.Util;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Framework;

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

            serviceRegistry.Register<IBlobContainerFactory, BlobContainerFactory>();
        }
    }
}