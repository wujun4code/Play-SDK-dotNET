using System;

namespace LeanCloud
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class PlayRPCAttribute : Attribute
    {

    }


    /// <summary>
    /// Defines the class name for a subclass of AVObject.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class PlaySynchronizeObjectNameAttribute : Attribute
    {
        /// <summary>
        /// Constructs a new ParseClassName attribute.
        /// </summary>
        /// <param name="className">The class name to associate with the AVObject subclass.</param>
        public PlaySynchronizeObjectNameAttribute(string className)
        {
            this.ClassName = className;
        }

        /// <summary>
        /// Gets the class name to associate with the AVObject subclass.
        /// </summary>
        public string ClassName { get; private set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class PlaySynchronizePropertyAttribute : Attribute
    {
        public PlaySynchronizePropertyAttribute(string keyName)
        {
            KeyName = keyName;
        }

        public string KeyName { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class PlayEventAttribute : Attribute
    {

    }
}
