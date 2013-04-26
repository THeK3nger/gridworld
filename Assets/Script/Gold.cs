using UnityEngine;
using System.Collections;

public class Gold : GridWorldBehaviour {

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(Vector3.up, 0.01f);
	}
}
