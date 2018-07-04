using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Weapon weapon;
    private new Collider2D collider;

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
    }
}
