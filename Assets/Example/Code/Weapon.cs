using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WeaponDefinition;

public class Weapon : SavedComponent
{
    [RuntimeSave]
    public WeaponDefinition def;

    [SerializeField, HideInInspector] private float muzzleGizmoSize = 0.1f;
    [SerializeField, HideInInspector] private int muzzleIndex = 0;


    public void Shoot(GameObject origin, Collider2D ignore = null)
    {
        if (!def)
            return;

        Vector2 muzzle = Vector2.zero;
        if (def.muzzles.Length > 0)
        {
            switch (def.muzzlemode)
            {
                case MuzzleMode.single:
                {
                    LaunchBullet(def.muzzles[0], origin, ignore);
                }
                break;
                case MuzzleMode.all:
                {
                    foreach (var item in def.muzzles)
                    {
                        LaunchBullet(item, origin, ignore);
                    }
                }
                break;
                case MuzzleMode.sequential:
                {
                    LaunchBullet(def.muzzles[muzzleIndex], origin, ignore);
                    muzzleIndex++;
                    if (muzzleIndex >= def.muzzles.Length)
                        muzzleIndex = 0;
                }
                break;
                case MuzzleMode.random:
                {
                    LaunchBullet(def.muzzles[Random.Range(0, def.muzzles.Length)], origin, ignore);
                }
                break;
                default:
                break;
            }
        }
    }

    private void LaunchBullet(Muzzle muzzle, GameObject origin, Collider2D ignore = null)
    {
        Vector2 worldPos = transform.TransformPoint(muzzle.pos);
        Vector2 direction = Rotate(transform.up, muzzle.angle);

        GameObject bullet = Instantiate(def.bulletPrefab, worldPos, Quaternion.FromToRotation(transform.up, direction) * transform.rotation);


        DamageOnCollision damageOnCollision = bullet.GetComponent<DamageOnCollision>();

        damageOnCollision.origin = origin;
        damageOnCollision.IgnoreCollisionWithOrigin = ignore != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (!def)
            return;
        foreach (var item in def.muzzles)
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(item.pos, Vector3.one * muzzleGizmoSize);
            Gizmos.DrawLine(item.pos, item.pos + (Rotate(Vector2.up, item.angle) * 0.3f));
        }
    }

    public Vector2 Rotate(Vector2 v, float angle)
    {
        float deg = angle * Mathf.Deg2Rad;
        float sin = Mathf.Sin(deg);
        float cos = Mathf.Cos(deg);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}
