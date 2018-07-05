using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour {

    public float value;

    private float created;

	// Use this for initialization
	void Start () {
        created = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {

		if(Time.timeSinceLevelLoad > created + value)
        {
            Destroy(gameObject);
        }
	}
}
