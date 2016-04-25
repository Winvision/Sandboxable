using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sandboxable.Hyak.Common.Internals
{
    public static class PortablePlatformAbstraction
    {
        private static readonly Dictionary<string, string> PlatformNamesAndAssemblies;

        private static readonly object _lock;

        private static readonly IDictionary<Assembly, Assembly> _platformAssemblies;

        private static readonly IDictionary<Type, Type> _platformAbstractions;

        static PortablePlatformAbstraction()
        {
            var dictionary = new Dictionary<string, string>
            {
                {"NetFramework", "Microsoft.Azure.Common.NetFramework"}
            };

            PlatformNamesAndAssemblies = dictionary;
            _lock = new object();
            _platformAssemblies = new Dictionary<Assembly, Assembly>();
            _platformAbstractions = new Dictionary<Type, Type>();
        }

        public static T Get<T>(bool hasDefaultImplementation = false)
        {
            T t;
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException();
            }
            if (!typeof(T).Name.StartsWith("I", (StringComparison)4))
            {
                throw new ArgumentException();
            }

            Type platformTypeFullName = null;

            lock (_lock)
            {
                if (!_platformAbstractions.TryGetValue(typeof(T), out platformTypeFullName))
                {
                    var callingAssembly = Assembly.GetCallingAssembly();
                    platformTypeFullName = GetPlatformTypeFullName<T>(GetPlatformAssembly(callingAssembly));

                    if (platformTypeFullName == null && hasDefaultImplementation)
                    {
                        platformTypeFullName = GetPlatformTypeFullName<T>(callingAssembly);
                    }

                    if (platformTypeFullName != null)
                    {
                        _platformAbstractions.Add(typeof(T), platformTypeFullName);
                    }
                }

                if (platformTypeFullName == null)
                {
                    throw new PlatformNotSupportedException();
                }
            }

            try
            {
                t = (T)Activator.CreateInstance(platformTypeFullName);
            }
            catch (Exception exception)
            {
                throw new PlatformNotSupportedException("...", exception);
            }

            return t;
        }

        private static Assembly GetPlatformAssembly(Assembly callingAssembly)
        {
            Assembly assembly;

            if (!_platformAssemblies.TryGetValue(callingAssembly, out assembly))
            {
                foreach (var platformNamesAndAssembly in PlatformNamesAndAssemblies)
                {
                    try
                    {
                        assembly = Assembly.Load(platformNamesAndAssembly.Value);
                        _platformAssemblies.Add(callingAssembly, assembly);
                        break;
                    }
                    catch (FileNotFoundException)
                    {
                    }
                }
            }

            if (assembly == null)
            {
                return Assembly.GetCallingAssembly();
            }

            return assembly;
        }

        private static Type GetPlatformTypeFullName<T>(Assembly platformAssembly)
        {
            return platformAssembly.GetTypes().Where(p =>
            {
                if (!typeof(T).IsAssignableFrom(p))
                {
                    return false;
                }

                return !p.IsInterface;
            }).Single();
        }
    }
}