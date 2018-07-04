

using System;
using System.Collections.Generic;
using FullSerializer;
using UnityEngine;

[fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
public class SavedComponent : MonoBehaviour
{
    [fsProperty] private int ownerInstanceID;

    public void SetOwnerInstanceID(int id)
    {
        this.ownerInstanceID = id;
    }
}

