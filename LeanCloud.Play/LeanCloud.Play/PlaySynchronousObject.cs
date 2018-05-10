using LeanCloud.Core.Internal;
using LeanCloud.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LeanCloud
{
    /// <summary>
    /// synchronous object in Play.
    /// </summary>
    public class PlaySynchronousObject : IEnumerable<KeyValuePair<string, object>>
    {
        internal readonly Object metaDataMutex = new Object();
        protected AVObject objectState;
        internal string ObjectName;
        internal virtual IDictionary<string, string> fixedKeys { get; set; }
        internal virtual IList<string> invalidKeys { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public PlaySynchronousObject()
        {
            invalidKeys = new List<string> { "cmd", "peerId", "op", "appId", "i" };
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            lock (metaDataMutex)
            {
                return ((IEnumerable<KeyValuePair<string, object>>)objectState).GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (metaDataMutex)
            {
                return ((IEnumerable<KeyValuePair<string, object>>)objectState).GetEnumerator();
            }
        }

        internal virtual IDictionary<string, object> CurrentData
        {
            get
            {
                return this.objectState.State.ToDictionary(x => x.Key, x => x.Value);
            }
        }

        internal virtual object this[string key]
        {
            get
            {
                return objectState[key];
            }
            set
            {
                objectState[key] = value;
            }
        }

        internal ICollection<string> Keys
        {
            get
            {
                lock (metaDataMutex)
                {
                    return objectState.Keys;
                }
            }
        }
        internal bool TryGetValue<T>(string key, out T result)
        {
            lock (metaDataMutex)
            {
                if (ContainsKey(key))
                {
                    try
                    {
                        var temp = Conversion.To<T>(this[key]);
                        result = temp;
                        return true;
                    }
                    catch (InvalidCastException ex)
                    {
                        result = default(T);
                        return false;
                    }
                }
                result = default(T);
                return false;
            }
        }
        public T Get<T>(string key)
        {
            return this.objectState.Get<T>(key);
        }

        public T GetValue<T>(string key)
        {
            var result = default(T);
            TryGetValue(key, out result);
            return result;
        }

        protected T GetProperty<T>(T defaultValue,
#if !UNITY
 [CallerMemberName] string propertyName = null
#else
 string propertyName
#endif
)
        {
            T result;
            var fieldName = GetFieldForPropertyName(ObjectName, propertyName);
            if (TryGetValue<T>(fieldName, out result))
            {
                return result;
            }
            return defaultValue;
        }
        private static string GetFieldForPropertyName(String className, string propertyName)
        {
            String fieldName = null;
            PlayCorePlugins.Instance.SynchronousObjectSubclassController.GetPropertyMappings(className).TryGetValue(propertyName, out fieldName);
            return fieldName;
        }

        protected void SetProperty<T>(T value,
#if !UNITY
 [CallerMemberName] string propertyName = null,
#else
 string propertyName,
#endif
         bool directSave = false
)
        {
            var fieldName = GetFieldForPropertyName(ObjectName, propertyName);
            this[fieldName] = value;
            if (directSave)
            {
                this.Save();
            }
        }

        internal bool ContainsKey(string key)
        {
            return this.objectState.ContainsKey(key);
        }

        internal virtual IDictionary<string, object> EncodeAttributes()
        {
            var currentOperations = objectState.StartSave();
            var jsonToSave = AVObject.ToJSONObjectForSaving(currentOperations);
            return jsonToSave;
        }

        /// <summary>
        /// call this method to notify players in the Room when properties updated.
        /// </summary>
        internal virtual void Save()
        {

        }

        internal virtual void Save(IDictionary<string, object> increment)
        {

        }

        internal virtual void SetServerData(IDictionary<string, object> metaData)
        {
            var fixedData = FixServerData(metaData);
            var serverState = AVObjectCoder.Instance.Decode(fixedData, AVDecoder.Instance);
            this.objectState = AVObject.FromState<AVObject>(serverState, this.ObjectName);
        }

        internal virtual void MergeFromServer(IDictionary<string, object> metaData)
        {
            var fixedData = FixServerData(metaData);
            var serverState = AVObjectCoder.Instance.Decode(fixedData, AVDecoder.Instance);
            this.objectState.MergeFromServer(serverState);
        }

        internal virtual IDictionary<string, object> FixServerData(IDictionary<string, object> metaData)
        {
            var filterKeysMetaData = metaData.Filter(this.invalidKeys);
            var fixedKeysMetaData = filterKeysMetaData.FixKeys(this.fixedKeys);
            return fixedKeysMetaData;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Hashtable CustomProperties
        {
            get
            {
                return this.CustomPropertiesMetaData.ToHashtable();
            }
            set
            {
                var dictionary = value.ToDictionary<string, object>();
                //this = this.CustomPropertiesMetaData.Merge(dictionary);
                this.Save(dictionary);
            }
        }

        private IDictionary<string, object> customProperties = new Dictionary<string, object>();

        internal IDictionary<string, object> CustomPropertiesMetaData
        {
            get
            {
                return customProperties;
            }
            set
            {
                customProperties = value;
            }
        }

        public DateTime? CreatedAt
        {
            get
            {
                return objectState.CreatedAt;
            }
        }

        public DateTime? UpdatedAt
        {
            get
            {
                return objectState.UpdatedAt;
            }
        }

        internal bool IsDirty
        {
            get
            {
                return objectState.IsDirty;
            }
        }
    }
}
