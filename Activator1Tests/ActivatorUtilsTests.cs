using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Activator1.Tests
{
    public interface I1
    {
        void SayHello();
    }

    [TestClass()]
    public class ActivatorUtilsTests
    {
        [ClassInitialize]
        public static void Init(TestContext testContext)
        {
            var ndirs = GetNumDirs();
            foreach (var dir in ndirs)
            {
                try
                {
                    Directory.Delete(dir);
                }
                catch (Exception)
                {
                }
            }

            ndirs = GetNumDirs();
            var num = ndirs.Any() ? int.Parse(ndirs.First()) : 1;
            var dirname = $"{num + 1:D4}";
            Directory.CreateDirectory(dirname);
            Directory.SetCurrentDirectory(dirname);
        }

        private static IEnumerable<string> GetNumDirs()
        {
            var currentDir = Directory.GetCurrentDirectory();
            return from dir in Directory.GetDirectories(currentDir)
                   let name = new DirectoryInfo(dir).Name
                   where name.Length == 4 && name.All(char.IsDigit)
                   orderby name descending
                   select name;
        }

        [TestMethod()]
        public void CreateInstanceFromTypeNameTest()
        {
            var dllname = "Generated.dll";
            MakeDll(dllname);
            File.Exists(dllname).Should().BeTrue();

            var asmName = Path.GetFileNameWithoutExtension(dllname);
            var xelement = new XElement("Ftp", new XAttribute("Class", $"X2, {asmName}"));
            var result = ActivatorUtils.CreateInstanceFromTypeName<I1>(xelement);
            result.Should().NotBeNull();
            (result is I1).Should().BeTrue();

            File.Delete(dllname);
            File.Exists(dllname).Should().BeFalse();
        }

        /// <summary>
        /// Generate DLL in a different directory - we should therefore not be able to load the type until we add the dir as an extra path
        /// </summary>
        [TestMethod()]
        public void CreateInstanceFromTypeNameTestOtherDir()
        {
            var otherDirname = "Other";
            var dllNameOtherDir = Path.Combine(otherDirname, "Generated2.dll");
            MakeDll(dllNameOtherDir);
            var asmName = Path.GetFileNameWithoutExtension(dllNameOtherDir);
            var xelement = new XElement("Ftp", new XAttribute("Class", $"X2, {asmName}"));
            var result = ActivatorUtils.CreateInstanceFromTypeName<I1>(xelement);
            result.Should().BeNull();

            var xelementWithExtraPath = new XElement("Ftp", new XAttribute("Class", $"X2, {asmName}"), new XAttribute("ExtraPaths", otherDirname));
            result = ActivatorUtils.CreateInstanceFromTypeName<I1>(xelementWithExtraPath);
            result.Should().NotBeNull();
            (result is I1).Should().BeTrue();
        }

        private void MakeDll(string name)
        {
            var dir = Path.GetDirectoryName(name);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var parameters = new CompilerParameters
            {
                GenerateExecutable = false,
                OutputAssembly = name
            };

            var fullname = Assembly.GetExecutingAssembly().Location;

            parameters.ReferencedAssemblies.Add(fullname);
            parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.dll");

            CompilerResults r = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(parameters,
                "using System; using System.Xml.Linq; using Activator1.Tests; public class X2 : I1 {public static int i=42; public X2(XElement e, params object[] p){} public void SayHello() {Console.WriteLine(\"Hello!\");}}");
        }
    }
}