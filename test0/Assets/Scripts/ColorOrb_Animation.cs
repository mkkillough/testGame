using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorOrb_Animation : MonoBehaviour {
    public float rotationX = 1;
    public float rotationY = 1;
    public float rotationZ = 1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.back, rotationZ);
        transform.Rotate(Vector3.up, rotationY);
        transform.Rotate(Vector3.right, rotationX);
    }
}
