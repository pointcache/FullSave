using System.Collections;
using System.Collections.Generic;
using pointcache.FullSave;
using UnityEngine;


/// <summary>
/// Shows how to interface with the SaveSystem
/// </summary>
public class SaveSystemDemo : MonoBehaviour
{
    private FullSaveSystem saveSystem;
    private string path;

    private void Awake()
    {
        saveSystem = GetComponent<FullSaveSystem>();
        path = Application.persistentDataPath + "/test/save.json";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)){
            saveSystem.Save(path);
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            saveSystem.Load(path);
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 30, 50, 60), "Press 1 to save");
        if (GUI.Button(new Rect(0, 0, 50, 30), "Save"))
        {
            saveSystem.Save(path);
        }

        GUI.Label(new Rect(60, 30, 50, 60), "Press 2 to load");
        if (GUI.Button(new Rect(60, 0, 50, 30), "Load"))
        {
            saveSystem.Load(path);
        }
    }
}
