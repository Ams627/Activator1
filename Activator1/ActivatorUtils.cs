using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Activator1
{
    public static class ActivatorUtils
    {
        private class Resolver
        {
            private string[] _paths;

            public Resolver(string path)
            {
                _paths = path.Split(';');
            }

            public Assembly AssemblyResolver(AssemblyName name)
            {
                System.Diagnostics.Debug.WriteLine($"{name}");
                var pathToLoad = _paths.Select(path => Path.Combine(path, $"{name}.dll")).Where(x => File.Exists(x)).FirstOrDefault();
                return pathToLoad == null ? null : Assembly.LoadFrom(pathToLoad);
            }
        }

        public static T CreateInstanceFromTypeName<T>(XElement classInfo, params object[] constructorParams) where T : class
        {
            var classname = classInfo?.Attribute("Class")?.Value;
            var cparams = new object[] { classInfo }.Concat(constructorParams).ToArray();
            var extraPaths = classInfo?.Attribute("ExtraPaths")?.Value;
            var type = extraPaths == null ? Type.GetType(classname) : Type.GetType(classname, new Resolver(extraPaths).AssemblyResolver, null, false);
            return type == null ? null : Activator.CreateInstance(type, cparams) as T;
        }
    }
}