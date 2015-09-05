using LightInject;

namespace BlogIndexer.Composition
{
    public class CompositionModule : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IndexerJob>();
        }
    }
}