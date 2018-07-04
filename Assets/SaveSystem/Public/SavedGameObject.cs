using UnityEngine;
using System.Collections;

public class SavedGameObject : MonoBehaviour
{
    [HideInInspector] public string _guid;

#if UNITY_EDITOR

    private void Reset()
    {
        for (int i = 0; i < 100; i++)
        {
            UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
        }
    }
#endif
}
