using LeanCloud.Storage.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LeanCloud.Internal
{
    public class PlaySynchronousObjectSubclassController
    {
        private readonly IDictionary<string, PlaySynchronousObjectSubclassInfo> registeredySynchronousObjects;
        private readonly ReaderWriterLockSlim mutex;

        public PlaySynchronousObjectSubclassController()
        {
            mutex = new ReaderWriterLockSlim();
            registeredySynchronousObjects = new Dictionary<string, PlaySynchronousObjectSubclassInfo>();
        }

        public void RegisterSubclass(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            if (!typeof(PlaySynchronousObject).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                throw new ArgumentException("Cannot register a type that is not a implementation of PlaySynchronousObject");
            }
            var className = GetClassName(type);
            try
            {
                mutex.EnterWriteLock();
                ConstructorInfo constructor = type.FindConstructor();
                if (constructor == null)
                {
                    throw new ArgumentException("Cannot register a type that does not implement the default constructor!");
                }
                var classInfo = new PlaySynchronousObjectSubclassInfo(type, constructor);

                registeredySynchronousObjects[className] = classInfo;
            }
            finally
            {
                mutex.ExitWriteLock();
            }
        }
        public String GetClassName(Type type)
        {
            return type == typeof(PlaySynchronousObject) ? "PlaySynchronousObject" : PlaySynchronousObjectSubclassInfo.GetSynchronousObjectClassName(type.GetTypeInfo());
        }

        public IDictionary<String, String> GetPropertyMappings(String className)
        {
            PlaySynchronousObjectSubclassInfo info = null;
            mutex.EnterReadLock();
            registeredySynchronousObjects.TryGetValue(className, out info);
            if (info == null)
            {
                registeredySynchronousObjects.TryGetValue("PlaySynchronousObject", out info);
            }
            mutex.ExitReadLock();
            return info.PropertyMappings;
        }
    }
}
