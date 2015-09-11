using LightInject;

namespace MiniBlog.ImageOptimizer.Composition
{
    public class CompositionModule : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<OptimizerJob>();
        }
    }
}