

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
        public static void Register_fsGameObjectConverter()
        {
            // do something here, ie:
            Converters.Add(typeof(fsGameObjectConverter));
        }
    }
}
namespace pointcache.FullSave.Internal
{
    /// <summary>
    /// Serializes GameObject reference fields into an id
    /// </summary>
    public class fsGameObjectConverter : fsDirectConverter
    {
        public override Type ModelType
        {
            get
            {
                return typeof(GameObject);
            }
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            if ((UnityEngine.GameObject)instance != null)
            {
                serialized = new fsData(((GameObject)instance).GetInstanceID());
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
                return null;

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

            return go;
        }

    }

}