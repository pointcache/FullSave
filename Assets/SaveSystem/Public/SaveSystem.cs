using System.Collections;
using System.Collections.Generic;
using System.IO;
using FullSerializer;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{

    public SaveSystemStorage storage;

    private Dictionary<string, GameObject> gameobjectsByGUID;
    private Dictionary<string, SavedScriptable> scriptablesByGUID;
    private fsSerializer serializer;
    private const string gosPath = "gameObjects.json";
    private const string compsPath = "components.json";


    private void Awake()
    {
        serializer = new fsSerializer();

        gameobjectsByGUID = new Dictionary<string, GameObject>();
        scriptablesByGUID = new Dictionary<string, SavedScriptable>();

        for (int i = 0; i < storage.objects.Count; i++)
        {
            SaveSystemStorage.ObjectData data = storage.objects[i];

            bool isgameobject = false;

            if (typeof(GameObject).IsAssignableFrom(data.target.GetType()))
            {
                isgameobject = true;
            }

            if (isgameobject)
            {
                gameobjectsByGUID.Add(data.guid, data.target as GameObject);
            }
            else
            {
                scriptablesByGUID.Add(data.guid, data.target as SavedScriptable);
            }

        }
    }

    public void Save(string path)
    {
        SerializationData serializationdata = new SerializationData();

        var targets = GameObject.FindObjectsOfType<SavedGameObject>();

        foreach (var item in targets)
        {
            serializationdata.gameObjects.Add(new GameObjectData(item));

            SavedComponent[] comps = item.GetComponents<SavedComponent>();
            foreach (var c in comps)
            {
                c.SetOwnerInstanceID(c.gameObject.GetInstanceID());
                serializationdata.components.Add(c);
            }
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }


        fsData data;
        serializer.TrySerialize(serializationdata.gameObjects, out data);

        StreamWriter sw = new StreamWriter(path + gosPath);
        sw.Write(fsJsonPrinter.PrettyJson(data));
        sw.Close();


        serializer.TrySerialize(serializationdata.components, out data);

        sw = new StreamWriter(path + compsPath);
        sw.Write(fsJsonPrinter.PrettyJson(data));
        sw.Close();
    }

    public void Load(string path)
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

    internal GameObject FindGameObject(string guid)
    {
        GameObject go;
        gameobjectsByGUID.TryGetValue(guid, out go);
        return go;
    }

}
