using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Activator1
{
    class X
    {
        public X(XElement e)
        {
            Console.WriteLine("Hello");
        }
    }
    static class ActivatorUtils
    {
        public static T CreateInstanceFromTypeName<T>(XElement classInfo, params object[] constructorParams) where T : class
        {
            var classname = classInfo?.Attribute("Class")?.Value;
            var cparams = new object[] { classInfo }.Concat(constructorParams).ToArray();
            var type = Type.GetType(classname);
            return Activator.CreateInstance(type, cparams) as T;
        }
    }
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var typename = typeof(X).AssemblyQualifiedName;
                var elem = new XElement("Ftp", new XAttribute("Class", typename));
                ActivatorUtils.CreateInstanceFromTypeName<X>(elem);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
                var progname = Path.GetFileNameWithoutExtension(fullname);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }
    }
}
