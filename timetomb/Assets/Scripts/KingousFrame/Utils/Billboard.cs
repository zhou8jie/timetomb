using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    public Transform target = null;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (this.target == null)
            return;
        transform.LookAt(this.target, Vector3.up);
	}
}
