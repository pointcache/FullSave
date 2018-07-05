using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnCollision : SavedComponent
{

    public float damage;
    public bool destroyAfterCollision;
    public bool IgnoreCollisionWithOrigin;
    [RuntimeSave] public GameObject origin;

    private new Collider2D collider;

    private void Start()
    {
        if (origin == null)
            return;

        if (IgnoreCollisionWithOrigin)
        {
            collider = GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(origin.GetComponent<Collider2D>(), collider);
        }
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
