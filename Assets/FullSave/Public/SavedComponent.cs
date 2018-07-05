#pragma warning disable  0414

using System;
using System.Collections.Generic;
using FullSave_FullSerializer;
using UnityEngine;


/// <summary>
/// Base class for components you want to save on runtime.
/// mark fields with [RuntimeSave] attribute
/// </summary>
[fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
public class SavedComponent : MonoBehaviour
{
    [RuntimeSave, SerializeField, DebugOnly] private int ownerInstanceID;

    public int fsOwnerInstanceID
    {
        get
        {
            return ownerInstanceID;
        }
        set
        {
            ownerInstanceID = value;
        }
    }

    public virtual void OnBeforeSave() { }

    public virtual void OnAfterLoad() { }
}

