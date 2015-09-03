using LightInject;
using MiniBlog.PostSync.Storage;

namespace MiniBlog.PostSync.Composition
{
    public class CompositionModule : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<ILocalStorage, LocalStorage>();
            serviceRegistry.Register<IChangeNotifier, FileBasedChangeNotifier>();
            serviceRegistry.Register<SyncJob>();
        }
    }
}