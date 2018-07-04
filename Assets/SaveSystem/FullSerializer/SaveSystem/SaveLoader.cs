

using System;
using System.Collections.Generic;
using System.IO;
using FullSerializer;
using UnityEngine;

public static class SaveLoader
{
    private static SaveSystem system;
    private static fsSerializer serializer;

    public static bool DeserializingComponent { get; set; }
    public static bool SerializingComponent { get; set; }

    public static Dictionary<int, GameObject> gameobjectsByInstanceID = new Dictionary<int, GameObject>();


    public static void Load(string[] serializedData, SaveSystem database, fsSerializer serializer)
    {
        SaveLoader.system = database;
        SaveLoader.serializer = serializer;


        List<GameObjectData> gameObjects = new List<GameObjectData>();

        serializer.TryDeserialize(fsJsonParser.Parse(serializedData[0]), ref gameObjects);

        SavedGameObject[] targets = GameObject.FindObjectsOfType<SavedGameObject>();
        gameobjectsByInstanceID.Clear();

        for (int i = targets.Length - 1; i >= 0; i--)
        {
            GameObject.Destroy(targets[i].gameObject);
        }

        Instantiation1(gameObjects);
        ComponentDeserialization(serializedData[1]);
    }

    private static void Instantiation1(List<GameObjectData> gameObjects)
    {
        foreach (GameObjectData data in gameObjects)
        {
            bool fromPrefab = !String.IsNullOrEmpty(data.guid);

            if (fromPrefab)
            {
                GameObject prefab = system.FindGameObject(data.guid);

                if (prefab != null)
                {
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
                        dgo._guid = data.guid;
                    }

                    go.transform.localScale = data.scale;
                    go.name = data.name;

                    gameobjectsByInstanceID.Add(data.InstanceID, go);
                }
                else
                {
                    Debug.LogError(String.Format("SaveSystem: While loading save file, gameobject with GUID{0} was not found in the database.", data.guid));
                    continue;
                }
            }

        }
    }

    private static void ComponentDeserialization(string data)
    {
        List<SavedComponent> components = new List<SavedComponent>();
        serializer.TryDeserialize(fsJsonParser.Parse(data), ref components);
    }
}
