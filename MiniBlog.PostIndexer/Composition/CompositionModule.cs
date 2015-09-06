using LightInject;

namespace MiniBlog.PostIndexer.Composition
{
    public class CompositionModule : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<Functions>();
        }
    }
}