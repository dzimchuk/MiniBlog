using LightInject;

namespace MiniBlog.Backup.Composition
{
    public class CompositionModule : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<BackupJob>();
        }
    }
}