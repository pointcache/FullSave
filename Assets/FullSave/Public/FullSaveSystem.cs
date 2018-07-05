namespace pointcache.FullSave
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using FullSave_FullSerializer;
    using pointcache.FullSave.Internal;
    using UnityEngine;

    /// <summary>
    /// Use this object to call Save() Load() and helper methods.
    /// </summary>
    public class FullSaveSystem : MonoBehaviour
    {
        /// <summary>
        /// Current storage asset
        /// </summary>
        public FullSaveStorage storage;

        private Dictionary<string, GameObject> prefabsByGUID;
        private Dictionary<string, SavedScriptable> scriptablesByGUID;
        private const string gosPath = "gameObjects.json";
        private const string compsPath = "components.json";
        private fsSerializer serializer;

        private bool requestedSave { get; set; }
        private string requestedSavePath { get; set; }

        private void Awake()
        {
            serializer = new fsSerializer();

            prefabsByGUID = new Dictionary<string, GameObject>();
            scriptablesByGUID = new Dictionary<string, SavedScriptable>();

            // Cache stored assets
            for (int i = 0; i < storage.objects.Count; i++)
            {
                FullSaveStorage.ObjectData data = storage.objects[i];

                bool isgameobject = false;

                if (typeof(GameObject).IsAssignableFrom(data.target.GetType()))
                {
                    isgameobject = true;
                }

                if (isgameobject)
                {
                    prefabsByGUID.Add(data.guid, data.target as GameObject);
                }
                else
                {
                    scriptablesByGUID.Add(data.guid, data.target as SavedScriptable);
                }

            }
        }

        public void Save(string path)
        {
            Save_Internal(path);

        }

        public void Load(string path)
        {
            Load_Internal(path);
        }

        /// <summary>
        /// Call this method to save whole scene into 2 files at specified path
        /// current limitation - you need to provide a directory as a path ending with `/`
        /// </summary>
        /// <param name="path"></param>
        private void Save_Internal(string path)
        {
            SerializationData serializationdata = new SerializationData();

            var targets = GameObject.FindObjectsOfType<SavedGameObject>();

            foreach (var item in targets)
            {
                serializationdata.gameObjects.Add(new GameObjectData(item));

                SavedComponent[] comps = item.GetComponents<SavedComponent>();
                foreach (var c in comps)
                {
                    c.fsOwnerInstanceID = c.gameObject.GetInstanceID();
                    serializationdata.components.Add(c);
                }
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }


            fsData data;

            serializer.TrySerialize(serializationdata.gameObjects, out data);

            using (var sw = new StreamWriter(path + gosPath))
            {
                sw.Write(fsJsonPrinter.PrettyJson(data));
            }

            data = null;
            serializer.TrySerialize(serializationdata.components, out data);

            using (var sw = new StreamWriter(path + compsPath))
            {
                sw.Write(fsJsonPrinter.PrettyJson(data));
            }

        }

        /// <summary>
        /// Call this method to load whole scene from 2 files at specified path
        /// current limitation - you need to provide a directory as a path ending with `/`
        /// </summary>
        /// <param name="path"></param>
        private void Load_Internal(string path)
        {
            string[] data = new string[2];

            using (var sr = new StreamReader(path + gosPath))
            {
                data[0] = sr.ReadToEnd();
            }

            using (var sr = new StreamReader(path + compsPath))
            {
                data[1] = sr.ReadToEnd();
            }


            SaveLoader.Load(data, this, serializer);

        }

        /// <summary>
        /// Retreives a prefab from storage cache by guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal GameObject FindPrefab(string guid)
        {
            GameObject go;
            prefabsByGUID.TryGetValue(guid, out go);
            return go;
        }

        /// <summary>
        /// Retreives a scriptable from storage cache by guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal SavedScriptable FindScriptable(string guid)
        {
            SavedScriptable so;
            scriptablesByGUID.TryGetValue(guid, out so);
            return so;
        }
    }

}