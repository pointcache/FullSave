using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodTestController : MonoBehaviour {

    public TestPODContainer copyfrom;
    public TestPODContainer copyto;

	// Use this for initialization
	void Start () {
        copyto.pod = copyfrom.pod;
	}
	
}
