using UnityEngine;
using System.Collections;

public class Roombadeliberator : MonoBehaviour, IBotDeliberator {
	
	private RoombaAction roombaState;
	
	// Use this for initialization
	void Start () {
		roombaState = gameObject.GetComponent<RoombaAction>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public string GetNextAction ()
	{
		string[] rooms = new string[]{"A","B","C","D","E","F"};
		Debug.Log(roombaState.GetState());
		return "move " + rooms[Random.Range(0,rooms.Length)];
	}

	public void NotifyObjectChange (GameObject obj, char type)
	{

	}

	public string interestType {
		get {
			return "";
		}
	}
}
