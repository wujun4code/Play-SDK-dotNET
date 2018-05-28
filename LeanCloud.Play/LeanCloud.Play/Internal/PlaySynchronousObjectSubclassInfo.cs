using LeanCloud.Storage.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud.Internal
{
    public class PlaySynchronousObjectSubclassInfo
    {
        public TypeInfo TypeInfo { get; private set; }
        public IDictionary<String, String> PropertyMappings { get; private set; }
        private ConstructorInfo Constructor { get; set; }

        public PlaySynchronousObjectSubclassInfo(Type type, ConstructorInfo constructor)
        {
            TypeInfo = type.GetTypeInfo();
            Constructor = constructor;
            PropertyMappings = ReflectionHelpers.GetProperties(type)
              .Select(prop => Tuple.Create(prop, prop.GetCustomAttribute<PlaySynchronizePropertyAttribute>(true)))
              .Where(t => t.Item2 != null)
              .Select(t => Tuple.Create(t.Item1, t.Item2.KeyName))
              .ToDictionary(t => t.Item1.Name, t => t.Item2);
        }

        public static string GetSynchronousObjectClassName(TypeInfo type)
        {
            var attribute = type.GetCustomAttribute<PlaySynchronizeObjectNameAttribute>();
            return attribute != null ? attribute.ClassName : null;
        }
    }
}
