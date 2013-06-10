using UnityEngine;
using System.Collections;

public class Roombadeliberator : MonoBehaviour, IBotDeliberator {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public string GetNextAction ()
	{
		string[] rooms = new string[]{"A","B","C","D","E","F"};
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
