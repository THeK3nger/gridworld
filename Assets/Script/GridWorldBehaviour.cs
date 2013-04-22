using UnityEngine;
using System.Collections;

public class GridWorldBehaviour : MonoBehaviour {

    protected GridWorldMap mapWorld;

	// Use this for initialization
	protected virtual void Awake () {
        mapWorld = GameObject.Find("MapGenerator").GetComponent<GridWorldMap>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
