using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystemDemo : MonoBehaviour
{

    private SaveSystem saveSystem;

    private void Awake()
    {
        saveSystem = GetComponent<SaveSystem>();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 50, 30), "Save"))
        {
            saveSystem.Save(Application.persistentDataPath + "/save/");
        }

        if (GUI.Button(new Rect(60, 0, 50, 30), "Load"))
        {
            saveSystem.Load(Application.persistentDataPath + "/save/");
        }
    }
}
