using Data;
using LightInject;
using MiniBlog.Contracts;
using Util;

public class CompositionModule : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.Register<IFileStorage, LocalFileStorage>();
        serviceRegistry.Register<IPostStorage, LocalPostStorage>();
        serviceRegistry.Register<IStorageAdapter, CachingPostStorage>();
        serviceRegistry.Register<IPostMapper, PostMapper>();

        serviceRegistry.Register<OptimizedImageService>(new PerContainerLifetime());
    }
}