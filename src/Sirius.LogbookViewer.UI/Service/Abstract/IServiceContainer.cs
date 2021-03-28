namespace Sirius.LogbookViewer.UI.Service
{
    /// <summary>
    /// Interface to use for the IoC container implementation to be defined in the executing assemblies.
    /// </summary>
    public interface IServiceContainer
    {
        void Register<S, T>() where T : S;
        void Register<S>();
        void Register<S>(object instance);
        T Resolve<T>();
    }
}
