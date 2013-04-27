using UnityEngine;
using System.Collections;

public class Gold : GridWorldBehaviour {

    private Spawner goldSpawner;

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
        goldSpawner = GameObject.Find("GoldSpawner").GetComponent<Spawner>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(Vector3.up, 0.01f);
	}

    void OnDestroy()
    {
        goldSpawner.KillItem();
    }


}
