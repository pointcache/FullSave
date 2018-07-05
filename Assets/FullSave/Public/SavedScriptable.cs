#pragma warning disable  0414

using FullSave_FullSerializer;
using UnityEngine;

/// <summary>
/// Base class for scriptables you want to save on runtime.
/// mark fields with [RuntimeSave] attribute
/// </summary>
[fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
public class SavedScriptable : ScriptableObject
{
    [RuntimeSave, SerializeField, HideInInspector] private string _guid;

    public void SetGuid(string id)
    {
        this._guid = id;
    } 

    public virtual void OnBeforeSave() { }

    public virtual void OnAfterLoad() { }
}