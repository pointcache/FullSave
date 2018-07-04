using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{

    public float damage;

    public bool destroyAfterCollision;

    public GameObject origin;

    public bool IgnoreCollisionWithOrigin;

    private new Collider2D collider;

    private void Start()
    {
        collider = GetComponent<Collider2D>();
        if (IgnoreCollisionWithOrigin)
            Physics2D.IgnoreCollision(origin.GetComponent<Collider2D>(), collider);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.ApplyDamage(damage, origin.gameObject);
        }

        if (destroyAfterCollision)
        {
            Destroy(gameObject);
        }
    }
}
