

using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComponentData
{
    public int ownerInstanceID;
    public Type type;
    public SavedComponent comp;

    public ComponentData(SavedComponent comp)
    {
        this.comp = comp;
        this.ownerInstanceID = ((MonoBehaviour)comp).gameObject.GetInstanceID();
        this.type = comp.GetType();
    }
}
