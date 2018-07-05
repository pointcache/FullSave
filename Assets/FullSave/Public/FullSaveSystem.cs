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


        /// <summary>
        /// Call this method to save whole scene into a file at specified path
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            Save_Internal(path);

        }
        /// <summary>
        /// Call this method to load whole scene from a file at specified path
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            Load_Internal(path);
        }


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

            string parent = Directory.GetParent(path).ToString();
            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }

            using (var sw = new StreamWriter(path))
            {
                fsData data;

                serializer.TrySerialize(serializationdata.gameObjects, out data);

                sw.Write(fsJsonPrinter.PrettyJson(data));
                sw.WriteLine(sw.NewLine);
                data = null;
                serializer.TrySerialize(serializationdata.components, out data);


                sw.Write(fsJsonPrinter.PrettyJson(data));

            }

        }


        private void Load_Internal(string path)
        {
            string[] data = new string[2];

            using (var sr = new StreamReader(path))
            {
                string alldata = sr.ReadToEnd();
                int breakIndex = alldata.IndexOf("]") + 1;


                data[0] = alldata.Substring(0, breakIndex);
                data[1] = alldata.Substring(breakIndex);
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