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
using System.Diagnostics;

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
            var dllname = "Generated.dll";
            MakeDll(dllname);
            var asmName = Path.GetFileNameWithoutExtension(dllname);
            var xelement = new XElement("Ftp", new XAttribute("Class", $"X2, {asmName}"));
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

            var fullname = Assembly.GetExecutingAssembly().Location;
            var dllname = Path.GetFileNameWithoutExtension(fullname);

            parameters.ReferencedAssemblies.Add(fullname);
            parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.dll");

            CompilerResults r = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(parameters,
                "using System; using System.Xml.Linq; using Activator1.Tests; public class X2 : I1 {public static int i=42; public X2(XElement e, params object[] p){} public void SayHello() {Console.WriteLine(\"Hello!\");}}");
        }
    }
}