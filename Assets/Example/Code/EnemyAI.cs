using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : SavedComponent
{
    [SerializeField] private Transform target;
    [SerializeField] private float shotDistance = 2f;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private bool CanShoot = true;

    [SerializeField, RuntimeSave] private float speed = 3f;

    private bool inRange;

    private Weapon weapon;
    private new Collider2D collider;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        StartCoroutine(Shoot());
    }

    // Update is called once per frame
    void Update()
    {

        if (!target)
        {
            target = GameObject.FindObjectOfType<Player>()?.transform;
        }

        if (!target)
            return;

        transform.up = target.position - transform.position;

        float distanceToTarget = Vector2.Distance(target.position, transform.position);

        inRange = distanceToTarget > shotDistance;
        if (inRange)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
        }
    }

    private IEnumerator Shoot()
    {
        while (true)
        {
            if (inRange && CanShoot)
            {
                weapon.Shoot(gameObject, collider);
            }
            yield return new WaitForSeconds(fireRate);
        }
    }
}
