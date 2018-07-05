using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pod
{
    public Color color;
}

public class TestPODContainer : SavedComponent
{

    [RuntimeSave] public Pod pod;

    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        sprite.color = pod.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        pod.color = Random.ColorHSV();
    }
}
