using System;
using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using FullSerializer.Internal;
using UnityEngine;

#if !UNITY_EDITOR && UNITY_WSA
// For System.Reflection.TypeExtensions
using System.Reflection;
#endif

namespace FullSerializer
{
    partial class fsConverterRegistrar
    {
        // Method 1: Via a field
        // Important: The name *must* begin with Register
        public static fsISavedComponentConverter Register_IdConverter;

        // Method 2: Via a method
        // Important: The name *must* begin with Register
        public static void Register_fsISavedComponentConverter()
        {
            // do something here, ie:
            Converters.Add(typeof(fsISavedComponentConverter));
        }
    }
}

public class fsISavedComponentConverter : fsConverter
{
    public override bool CanProcess(Type type)
    {
        return typeof(SavedComponent).IsAssignableFrom(type);
    }

    public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
    {
        serialized = fsData.CreateDictionary();
        var result = fsResult.Success;

        // bool deserializingThis = false;
        // if (!SaveLoader.DeserializingComponent)
        // {
        //     SaveLoader.DeserializingComponent = true;
        //     deserializingThis = true;
        // }

        fsMetaType metaType = fsMetaType.Get(Serializer.Config, instance.GetType());
        metaType.EmitAotData(/*throwException:*/ false);

        for (int i = 0; i < metaType.Properties.Length; ++i)
        {
            fsMetaProperty property = metaType.Properties[i];

            if (property.CanRead == false && property.RuntimeSave == false)
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

        // if (deserializingThis)
        // {
        //     SaveLoader.DeserializingComponent = false;
        // }

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



    public override object CreateInstance(fsData data, Type storageType)
    {
        if (!data.IsDictionary)
        {
            UnityEngine.Debug.LogError("SaveSystem: tried to deserialize component, which data layout was not a dictionary.");
            return 0;
        }

        Dictionary<string, fsData> keyValuePairs = data.AsDictionary;


        fsData ownerIDData;
        keyValuePairs.TryGetValue("ownerInstanceID", out ownerIDData);

        if (ownerIDData == null || !ownerIDData.IsInt64)
        {
            UnityEngine.Debug.LogError("SaveSystem: could not locate ownerID in a serialized component data.");
            return 0;
        }

        long ownerID = ownerIDData.AsInt64;

        GameObject go;
        if(!SaveLoader.gameobjectsByInstanceID.TryGetValue((int)ownerID, out go))
        {
            UnityEngine.Debug.LogError("SaveSystem: gameobject with such ID was not found.");
            return 0;
        }

        object component = go.GetComponent(storageType);
        if(component == null)
        {
            component = go.AddComponent(storageType);
        }

        return component;
    }
}
