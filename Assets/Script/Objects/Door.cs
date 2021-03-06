using UnityEngine;
using System.Collections;

public class Door : GridWorldBehaviour {

	public bool isOpen = false;

	private AstarPath astarpath;
    private SmartObjects smartObject;

	// Use this for initialization
	void Start () {
		GameObject astar = GameObject.Find ("A*");
		astarpath = astar.GetComponent<AstarPath> ();
        smartObject = gameObject.GetComponent<SmartObjects>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown() {
		Debug.Log("ClickableDoor");
		if (isOpen) {
			this.CloseDoor();
		} else {
			this.OpenDoor();
		}
	}

	public void OpenDoor() {
		Vector3 old_position = gameObject.transform.position;
		float x = old_position.x;
		float y = old_position.y-1.8f;
		float z = old_position.z;
		gameObject.transform.position = new Vector3(x,y,z);
		gameObject.layer = 8; // Walkable
		isOpen = true;
		astarpath.Scan();
        mapWorld.SetMapElement(x, z, '.');
        smartObject.NotifyStateChange();
	}

	public void CloseDoor() {
		Vector3 old_position = gameObject.transform.position;
		float x = old_position.x;
		float y = old_position.y+1.8f;
		float z = old_position.z;
		gameObject.transform.position = new Vector3(x,y,z);
		gameObject.layer = 9; // Obstacle
		isOpen = false;
		astarpath.Scan();
        mapWorld.SetMapElement(x, z, 'D');
        smartObject.NotifyStateChange();
	}

}
