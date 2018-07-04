

using System;
using System.Collections.Generic;
using FullSerializer;
using UnityEngine;

[System.Serializable]
public class GameObjectData
{
    public int InstanceID;
    public int ParentInstanceID;
    public string guid;
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public GameObjectData(SavedGameObject dgo)
    {
        InstanceID = dgo.gameObject.GetInstanceID();

        Transform tr = dgo.transform;
        position = tr.position;
        rotation = tr.rotation;
        scale = tr.localScale;

        if (dgo.transform.parent != null)
        {
            ParentInstanceID = tr.parent.GetInstanceID();
        }
        guid = dgo._guid;
        name = dgo.name;
    }
}
