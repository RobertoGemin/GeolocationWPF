using Unity;
using Unity.Lifetime;

namespace GeolocationApp
{
    public class DependencyInjector
    {
        private static readonly UnityContainer unityContainer = new UnityContainer();

        public static void Register<I, T>() where T : I
        {
            unityContainer.RegisterType<I, T>(new ContainerControlledLifetimeManager());
        }

        public static T Retrieve<T>()
        {
            return unityContainer.Resolve<T>();
        }

        public static void Inject<I>(I instance)
        {
            unityContainer.RegisterInstance(instance, new ContainerControlledLifetimeManager());
        }
    }
}