using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DebugOnlyAttribute))]
public class DebugOnlyPropertyDrawer : PropertyDrawer
{

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        InspectorMode inspectorMode = ActiveEditorTracker.sharedTracker.inspectorMode;

        if (inspectorMode == InspectorMode.Debug || inspectorMode == InspectorMode.DebugInternal)
        {
            base.OnGUI(position, property, label);
        }
        else
        {

        }
    }
}
