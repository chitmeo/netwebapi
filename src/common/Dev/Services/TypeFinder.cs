using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dev.Services;

public class TypeFinder : ITypeFinder
{
    #region Constants
    protected const string ASSEMBLY_SKIP_LOADING_PATTERN = "^System|^mscorlib|^Microsoft|^AjaxControlToolkit|^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EntityFramework|^EPPlus|^FluentValidation|^ImageResizer|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math|^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion|^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider|^WebActivator|^WebDev|^WebGrease";
    #endregion
    #region Fields

    protected static readonly Dictionary<string, Assembly> _assemblies = new(StringComparer.InvariantCultureIgnoreCase);

    protected static bool _loaded;
    protected static readonly object _locker = new();

    private readonly IDevFileProvider _fileProvider;

    #endregion

    #region Ctor

    public TypeFinder()
    {
        _fileProvider = CommonHelper.DefaultFileProvider;
    }

    static TypeFinder()
    {
        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
    }

    #endregion

    #region Utilities

    private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
        var assembly = args.LoadedAssembly;

        if (assembly.FullName == null)
            return;

        if (_assemblies.ContainsKey(assembly.FullName))
            return;

        if (!Matches(assembly.FullName))
            return;

        _assemblies.TryAdd(assembly.FullName, assembly);
    }

    protected static bool Matches(string assemblyFullName)
    {
        return !Regex.IsMatch(assemblyFullName, ASSEMBLY_SKIP_LOADING_PATTERN, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
    {
        try
        {
            var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
            return type.FindInterfaces((_, _) => true, null)
                .Where(implementedInterface => implementedInterface.IsGenericType).Any(implementedInterface =>
                    genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition()));
        }
        catch
        {
            return false;
        }
    }

    protected virtual IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
    {
        var result = new List<Type>();

        try
        {
            foreach (var a in assemblies)
            {
                Type[] types = null;
                try
                {
                    types = a.GetTypes();
                }
                catch
                {
                    //ignore
                }

                if (types == null)
                    continue;

                foreach (var t in types)
                {
                    if (!assignTypeFrom.IsAssignableFrom(t) && (!assignTypeFrom.IsGenericTypeDefinition || !DoesTypeImplementOpenGeneric(t, assignTypeFrom)))
                        continue;

                    if (t.IsInterface)
                        continue;

                    if (onlyConcreteClasses)
                    {
                        if (t.IsClass && !t.IsAbstract)
                            result.Add(t);
                    }
                    else
                        result.Add(t);
                }
            }
        }
        catch (ReflectionTypeLoadException ex)
        {
            var msg = string.Empty;

            if (ex.LoaderExceptions.Any())
                msg = ex.LoaderExceptions.Where(e => e != null)
                    .Aggregate(msg, (current, e) => $"{current}{e.Message + Environment.NewLine}");

            var fail = new Exception(msg, ex);
            Debug.WriteLine(fail.Message, fail);

            throw fail;
        }

        return result;
    }

    protected virtual void InitData()
    {
        //data already loaded
        if (_loaded)
            return;

        //prevent multi loading data
        lock (_locker)
        {
            //data can be loaded while we waited
            if (_loaded)
                return;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName == null)
                    continue;

                if (!Matches(assembly.FullName))
                    continue;

                _assemblies.TryAdd(assembly.FullName, assembly);
            }

            foreach (var directoriesToLoadAssembly in DirectoriesToLoadAssemblies)
            {
                if (!_fileProvider.DirectoryExists(directoriesToLoadAssembly))
                    continue;

                foreach (var dllPath in _fileProvider.GetFiles(directoriesToLoadAssembly, "*.dll"))
                    try
                    {
                        var an = AssemblyName.GetAssemblyName(dllPath);

                        if (_assemblies.ContainsKey(an.FullName))
                            continue;

                        if (!Matches(an.FullName))
                            continue;

                        Assembly assembly;

                        try
                        {
                            assembly = AppDomain.CurrentDomain.Load(an);
                        }
                        catch
                        {
                            assembly = Assembly.LoadFrom(dllPath);
                        }

                        _assemblies.TryAdd(assembly.FullName, assembly);
                    }
                    catch (BadImageFormatException ex)
                    {
                        Trace.TraceError(ex.ToString());
                    }
            }

            _loaded = true;
        }
    }

    #endregion

    #region Methods

    public virtual IList<Assembly> GetAssemblies()
    {
        if (!_loaded)
            InitData();

        return _assemblies.Values.ToList();
    }

    public virtual IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
    {
        return FindClassesOfType(typeof(T), onlyConcreteClasses);
    }

    public virtual IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
    {
        return FindClassesOfType(assignTypeFrom, GetAssemblies(), onlyConcreteClasses);
    }

    public virtual Assembly GetAssemblyByName(string assemblyFullName)
    {
        if (!_loaded)
            InitData();

        _assemblies.TryGetValue(assemblyFullName, out var assembly);

        if (assembly != null)
            return assembly;

        var assemblyName = new AssemblyName(assemblyFullName);
        var key = _assemblies.Keys.FirstOrDefault(k => k.StartsWith(assemblyName.Name ?? assemblyFullName.Split(' ')[0], StringComparison.InvariantCultureIgnoreCase));

        return string.IsNullOrEmpty(key) ? null : _assemblies[key];
    }

    #endregion

    #region Properties

    public virtual List<string> DirectoriesToLoadAssemblies { get; set; } = new()
    {
        AppContext.BaseDirectory
    };

    #endregion
}
