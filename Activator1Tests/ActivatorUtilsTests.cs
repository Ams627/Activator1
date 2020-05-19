using Microsoft.VisualStudio.TestTools.UnitTesting;
using Activator1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Xml.Linq;
using System.IO;

namespace Activator1.Tests
{
    public interface I1
    {
        void SayHello();
    }

    [TestClass()]
    public class ActivatorUtilsTests
    {
        [TestMethod()]
        public void CreateInstanceFromTypeNameTest()
        {
            var dllname = "testdll";
            MakeDll(dllname);
            var xelement = new XElement("Ftp", new XAttribute("Class", $"X2, {dllname}"));
            var result = ActivatorUtils.CreateInstanceFromTypeName<I1>(xelement);
            result.SayHello();
        }

        private void MakeDll(string name)
        {
            var parameters = new CompilerParameters
            {
                GenerateExecutable = false,
                OutputAssembly = name,
            };

            var fullname = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var dllname = Path.GetFileNameWithoutExtension(fullname);

            parameters.ReferencedAssemblies.Add(fullname);

            CompilerResults r = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(parameters,
                "using System; using Activator1.Tests; public class X2 : I1 {public static int i=42; public void SayHello() {Console.WriteLine(\"Hello!\");}}");

        }
    }
}