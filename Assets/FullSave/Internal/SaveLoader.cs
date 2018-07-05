namespace pointcache.FullSave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using FullSave_FullSerializer;
    using UnityEngine;

    /// <summary>
    /// Handles deserialization of gameObjects and SavedComponents
    /// </summary>
    public static class SaveLoader
    {
        private static FullSaveSystem system;
        public static FullSaveSystem System => system;
        private static fsSerializer serializer;
        private static List<GameObject> allGameObjects;

        public static Dictionary<int, GameObject> gameobjectsByInstanceID = new Dictionary<int, GameObject>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializedData">0 - gameobjects, 1 - components</param>
        /// <param name="system"></param>
        /// <param name="serializer"></param>
        public static void Load(string[] serializedData, FullSaveSystem system, fsSerializer serializer)
        {
            SaveLoader.system = system;
            SaveLoader.serializer = serializer;

            gameobjectsByInstanceID.Clear();
            allGameObjects = new List<GameObject>();

            List<GameObjectData> gameObjects = new List<GameObjectData>();

            serializer.TryDeserialize(fsJsonParser.Parse(serializedData[0]), ref gameObjects);

            // Clean up all existing gos
            SavedGameObject[] targets = GameObject.FindObjectsOfType<SavedGameObject>();
            for (int i = targets.Length - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(targets[i].gameObject);
            }

            GameObjectDeserialization(gameObjects);
            ComponentDeserialization(serializedData[1]);
            GameObjectActivation();
        }

        private static void GameObjectDeserialization(List<GameObjectData> gameObjects)
        {
            foreach (GameObjectData data in gameObjects)
            {
                // TODO: handle dynamic gameObjects
                bool fromPrefab = !String.IsNullOrEmpty(data.guid);

                if (fromPrefab)
                {
                    GameObject prefab = system.FindPrefab(data.guid);

                    if (prefab != null)
                    {
                        // We disable prefab to postpone unity callbacks
                        bool initState = prefab.activeSelf;
                        prefab.SetActive(false);

                        GameObject go = GameObject.Instantiate(prefab, data.position, data.rotation);
                        SavedGameObject dgo = go.GetComponent<SavedGameObject>();

                        if (dgo == null)
                        {
                            Debug.LogError(String.Format("SaveSystem: While loading save file, gameobject with GUID{0} did not have a DatabaseGameObject component, the object will be destroyed.", data.guid));

                            GameObject.Destroy(go);
                            continue;
                        }
                        else
                        {
                            dgo.guid = data.guid;
                        }

                        go.transform.localScale = data.scale;
                        go.name = data.name;

                        gameobjectsByInstanceID.Add(data.InstanceID, go);
                        allGameObjects.Add(go);

                        prefab.SetActive(initState);
                    }
                    else
                    {
                        Debug.LogError(String.Format("SaveSystem: While loading save file, gameobject with GUID{0} was not found in the database.", data.guid));
                        continue;
                    }
                }

            }
        }

        /// <summary>
        /// All chores about linking and deserialzing components we leave to fsSavedComponentConverter
        /// </summary>
        /// <param name="data"></param>
        private static void ComponentDeserialization(string data)
        {
            List<SavedComponent> components = new List<SavedComponent>();
            serializer.TryDeserialize(fsJsonParser.Parse(data), ref components);
        }

        private static void GameObjectActivation()
        {
            foreach (var item in allGameObjects)
            {
                item.SetActive(true);
            }
        }

    }

}