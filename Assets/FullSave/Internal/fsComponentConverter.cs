

using System;
using System.Collections.Generic;
using FullSave_FullSerializer;
using FullSave_FullSerializer.Internal.Converters;
using pointcache.FullSave.Internal;
using UnityEngine;

namespace FullSave_FullSerializer
{
    partial class fsConverterRegistrar
    {
        // Important: The name *must* begin with Register
        public static void Register_fsComponentConverter()
        {
            // do something here, ie:
            Converters.Add(typeof(fsComponentConverter));
        }
    }
}


namespace pointcache.FullSave.Internal
{

    /// <summary>
    /// Serializes UnityEngine.Component into gameobject ID 
    /// </summary>
    public class fsComponentConverter : fsConverter
    {

        public override bool CanProcess(Type type)
        {
            return typeof(UnityEngine.Component).IsAssignableFrom(type);
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            if ((UnityEngine.Component)instance != null)
            {
                serialized = new fsData(((UnityEngine.Component)instance).gameObject.GetInstanceID());
            }
            else
                serialized = fsData.Null;
            return fsResult.Success;
        }

        public override bool RequestCycleSupport(Type storageType)
        {
            return false;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {

            return fsResult.Success;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            if (data.IsNull)
            {
                return null;
            }

            if (!data.IsInt64)
            {
                UnityEngine.Debug.LogError("SaveSystem: gameobject reference data type mismatch.");
                return 0;
            }

            GameObject go;
            if (!SaveLoader.gameobjectsByInstanceID.TryGetValue((int)data.AsInt64, out go))
            {
                UnityEngine.Debug.LogError("SaveSystem: gameobject with such ID was not found.");
                return 0;
            }

            object component = go.GetComponent(storageType);
            if (component == null)
            {
                component = go.AddComponent(storageType);
            }

            return component;

        }

    }

}