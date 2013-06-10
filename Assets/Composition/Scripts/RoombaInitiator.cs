using UnityEngine;
using System.Collections;

public class RoombaInitiator : MonoBehaviour {
	
	public GameObject roomba;

	// Use this for initialization
	void Start () {
		Object roomba1 = Instantiate(roomba, new Vector3(3.5f, 0.1f, 7.5f), roomba.transform.rotation);
		Object roomba2 = Instantiate(roomba, new Vector3(3.5f, 0.1f, 16.5f), roomba.transform.rotation);
		roomba1.name = "Roomba1";
		roomba2.name = "Roomba2";
		GameObject.Find("Roomba1").GetComponent<RoombaAction>().currentRoom = "A";
		GameObject.Find("Roomba2").GetComponent<RoombaAction>().currentRoom = "B";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
