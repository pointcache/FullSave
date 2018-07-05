

using System;
using System.Collections.Generic;
using FullSave_FullSerializer;
using FullSave_FullSerializer.Internal;
using pointcache.FullSave.Internal;
using UnityEngine;

namespace FullSave_FullSerializer
{
    partial class fsConverterRegistrar
    {
        // Important: The name *must* begin with Register
        public static void Register_fsSavedScriptableConverter()
        {
            // do something here, ie:
            Converters.Add(typeof(fsSavedScriptableConverter));
        }
    }
}

namespace pointcache.FullSave.Internal
{

    /// <summary>
    /// Serializes SavedScriptableObjects
    /// </summary>
    public class fsSavedScriptableConverter : fsConverter
    {

        public override bool CanProcess(Type type)
        {
            return typeof(SavedScriptable).IsAssignableFrom(type);
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            serialized = fsData.CreateDictionary();
            var result = fsResult.Success;

            fsMetaType metaType = fsMetaType.Get(Serializer.Config, instance.GetType());
            metaType.EmitAotData(/*throwException:*/ false);

            for (int i = 0; i < metaType.Properties.Length; ++i)
            {
                fsMetaProperty property = metaType.Properties[i];

                if (property.RuntimeSave == false)
                    continue;

                fsData serializedData;

                var itemResult = Serializer.TrySerialize(property.StorageType, property.OverrideConverterType,
                                                         property.Read(instance), out serializedData);
                result.AddMessages(itemResult);
                if (itemResult.Failed)
                {
                    continue;
                }

                serialized.AsDictionary[property.JsonName] = serializedData;
            }
            return result;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            var result = fsResult.Success;

            // Verify that we actually have an Object
            if ((result += CheckType(data, fsDataType.Object)).Failed)
            {
                return result;
            }

            fsMetaType metaType = fsMetaType.Get(Serializer.Config, storageType);
            metaType.EmitAotData(/*throwException:*/ false);

            for (int i = 0; i < metaType.Properties.Length; ++i)
            {
                fsMetaProperty property = metaType.Properties[i];
                if (property.CanWrite == false)
                    continue;

                fsData propertyData;
                if (data.AsDictionary.TryGetValue(property.JsonName, out propertyData))
                {
                    object deserializedValue = null;

                    // We have to read in the existing value, since we need to
                    // support partial deserialization. However, this is bad for
                    // perf.
                    // TODO: Find a way to avoid this call when we are not doing
                    //       a partial deserialization Maybe through a new
                    //       property, ie, Serializer.IsPartialSerialization,
                    //       which just gets set when starting a new
                    //       serialization? We cannot pipe the information
                    //       through CreateInstance unfortunately.
                    if (property.CanRead)
                    {
                        deserializedValue = property.Read(instance);
                    }

                    var itemResult = Serializer.TryDeserialize(propertyData, property.StorageType,
                                                               property.OverrideConverterType, ref deserializedValue);
                    result.AddMessages(itemResult);
                    if (itemResult.Failed)
                        continue;

                    property.Write(instance, deserializedValue);
                }
            }

            return result;
        }

        /// <summary>
        /// Same as with gameObjects in SaveLoader, we lookup originals by guid in Storage, and 
        /// overwrite those.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="storageType"></param>
        /// <returns></returns>
        public override object CreateInstance(fsData data, Type storageType)
        {
            if (!data.IsDictionary)
            {
                UnityEngine.Debug.LogError("SaveSystem: tried to deserialize scriptable, which data layout was not a dictionary.");
                return 0;
            }

            Dictionary<string, fsData> keyValuePairs = data.AsDictionary;

            fsData guidData;
            keyValuePairs.TryGetValue("_guid", out guidData);

            if (guidData == null || !guidData.IsString)
            {
                UnityEngine.Debug.LogError("SaveSystem: could not scriptable in database using guid.");
                return 0;
            }

            string guid = guidData.AsString;

            SavedScriptable so = SaveLoader.System.FindScriptable(guid);
            if (so == null)
            {
                UnityEngine.Debug.LogError("SaveSystem: scriptable object with such guid was not found.");
                return 0;
            }

            return ScriptableObject.Instantiate(so);
        }
    }

}