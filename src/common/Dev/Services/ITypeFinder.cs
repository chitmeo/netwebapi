using System.Reflection;

namespace Dev.Services;

public interface ITypeFinder
{
    IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);
    IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);
    IList<Assembly> GetAssemblies();
    Assembly GetAssemblyByName(string assemblyFullName);
}
