namespace pointcache.FullSave
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;

    /// <summary>
    /// Collects and stores prefabs with SavedGameObject
    /// and scriptables derived from SavedScriptable
    /// </summary>
    [CreateAssetMenu(fileName = "FullSaveStorage", menuName = "FullSave/Create storage")]
    public class FullSaveStorage : ScriptableObject
    {

        [System.Serializable]
        public class ObjectData
        {
            public UnityEngine.Object target;
            public string guid;
        }

        public List<ObjectData> objects = new List<ObjectData>();

#if UNITY_EDITOR
        [MenuItem("FullSave/StoreObjects")]
        public static void StoreObjects()
        {
            FullSaveStorage database = GetAllAssetsAtPath<FullSaveStorage>("", true)[0];
            if (database == null)
            {
                Debug.LogError("Create a FullSaveStorage");
                return;
            }

            database.objects.Clear();

            // Store scriptables
            {
                SavedScriptable[] scriptables = GetAllAssetsAtPath<SavedScriptable>("", true);

                foreach (var s in scriptables)
                {
                    string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(s));
                    s.SetGuid(guid);
                    database.RegisterObject(s, guid);
                    EditorUtility.SetDirty(s);
                }

            }

            // Store prefabs
            {
                GameObject[] prefabs = GetAllGameObjects();
                foreach (var p in prefabs)
                {
                    SavedGameObject dgo = p.GetComponent<SavedGameObject>();
                    if (dgo)
                    {
                        string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(p));
                        dgo.guid = guid;
                        database.RegisterObject(p, guid);
                        EditorUtility.SetDirty(dgo.gameObject);
                    }
                }
            }
        }

        private void RegisterObject(UnityEngine.Object target, string guid)
        {
            ObjectData data = new ObjectData()
            {
                target = target,
                guid = guid
            };

            objects.Add(data);
        }

        private static bool HasAttribute(Type info, Type attributeType)
        {
            return info.GetCustomAttributes(attributeType, false).Length > 0;
        }

        private static T[] GetAllAssetsAtPath<T>(string path, bool globalSearch = false) where T : UnityEngine.Object
        {

            string type = typeof(T).FullName;

            var arr = globalSearch ? AssetDatabase.FindAssets("t:" + type) : AssetDatabase.FindAssets("t:" + type, new string[] { path });

            List<T> list = new List<T>();

            foreach (var item in arr)
            {
                T obj = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(item));
                if (obj != null)
                    list.Add(obj);
            }

            return list.ToArray();

        }

        private static GameObject[] GetAllGameObjects()
        {

            var arr = AssetDatabase.FindAssets("t:GameObject");

            List<GameObject> list = new List<GameObject>();

            foreach (var item in arr)
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(item));
                if (obj != null)
                    list.Add(obj);
            }

            return list.ToArray();

        }
#endif
    }

}