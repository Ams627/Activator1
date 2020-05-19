using System;
using System.Linq;
using System.Xml.Linq;

namespace Activator1
{
    public static class ActivatorUtils
    {
        public static T CreateInstanceFromTypeName<T>(XElement classInfo, params object[] constructorParams) where T : class
        {
            var classname = classInfo?.Attribute("Class")?.Value;
            var cparams = new object[] { classInfo }.Concat(constructorParams).ToArray();
            var type = Type.GetType(classname);
            return Activator.CreateInstance(type, cparams) as T;
        }
    }
}
