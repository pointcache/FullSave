using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    [SerializeField] private WeaponDefinition upgrade;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            var weapon = collision.gameObject.GetComponent<Weapon>();
            weapon.def = upgrade;
            Destroy(gameObject);
        }
    }
}
