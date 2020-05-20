using System;
using System.IO;
using System.Xml.Linq;

namespace Activator1
{
    internal class Program
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