using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb;
    public float Speed = 10;
    public float MaxSpeed = 50;
    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        Vector2 force = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            force += Vector2.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            force += Vector2.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            force += Vector2.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            force += Vector2.right;
        }

        Vector2.ClampMagnitude(force, 1f);

        rb.AddForce(force * Speed * Time.deltaTime, ForceMode2D.Force);

        Vector2 vel = rb.velocity;
        float mag = vel.magnitude;

        if (mag > MaxSpeed)
        {
            Vector2 compensation = -(vel.normalized * (MaxSpeed - mag));
            rb.velocity -= compensation;
        }
    }

    private void Rotate()
    {
        Vector2 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.up = mp - (Vector2)transform.position;
    }
}
