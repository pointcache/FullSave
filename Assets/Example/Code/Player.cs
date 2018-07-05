using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SavedComponent
{
    private Weapon weapon;
    private new Collider2D collider;

    [RuntimeSave, SerializeField]
    private Transform testComponentReference;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        weapon = GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            weapon.Shoot(gameObject, collider);
        }

    }

    private void OnGUI()
    {
        GUI.Label(new Rect(50, Screen.height - 50, 200, 50), GetComponent<Health>().Value.ToString());
    
        //if(GUI.Button(new Rect(200, Screen.height - 50, 200, 50), "Add"))
        //{
        //    var pod = new TestPODContainer.Pod()
        //    {
        //        Name = "hey hey hey!"
        //    };
        //    GetComponent<TestPODContainer>().pod = pod;
        //
        //    var enemies = GameObject.FindObjectsOfType<EnemyAI>();
        //    foreach (var item in enemies)
        //    {
        //        item.GetComponent<TestPODContainer>().pod = pod;
        //    }
        //}
    }
}
