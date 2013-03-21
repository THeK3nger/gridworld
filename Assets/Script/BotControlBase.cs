using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Basic implementation fo IBotControl.
 * 
 * \author Davide Aversa
 * \version 1.0
 * \date 2013
 */
public class BotControlBase : MonoBehaviour, IBotControl {

	private char[] myMap;			//Store local map perception.
	private int rsize, csize;		//Map size.
	private GridWorldMap mapworld;	//A reference to the original map.
	private BotActions botActions;  //Reference to the BotAction component.
	
	private List<GameObject> objectInFov; // Contains the list of object in the FOV.

	// CONDITIONS TODO: to be defined
	private bool grabbing = false;
	private bool test1 = true;

	// Use this for initialization
	void Awake() {
		mapworld = GameObject.Find("MapGenerator").GetComponent<GridWorldMap>();
		int[] sizes = mapworld.getMapSize ();
		rsize = sizes [0];
		csize = sizes [1];
		myMap = new char[rsize * csize];
		objectInFov = new List<GameObject>();
		botActions = gameObject.GetComponent<BotActions> ();
		// Run Thread Function Every 1 second
		InvokeRepeating("test", 3, 1);
	}
	 
	// Update is called once per frame
	void Update () {
	 
	}

	// Implementation of IBotBrain.objectEnteringFOV
	public void objectEnteringFOV(GameObject obj) {
	 	// Extract Type and update the map.
		SmartObjects attributes = obj.GetComponent<SmartObjects> ();
		char type = attributes.type[0];
		int idx = mapworld.getArrayIndexFromWorld (obj.transform.position.x, obj.transform.position.z);
		objectInFov.Add (obj);
		myMap [idx] = type;		
	}

	// Implementation of IBotBrain.objectLeavingFOV
	public void objectLeavingFOV(GameObject obj) {
		objectInFov.Remove (obj);
	}

	// Implementation of IBotBrain.CheckCondition
	public bool CheckCondition(string condition) {
		// PARSE AND
		string[] andConditions = condition.Split('&');
		if (andConditions.Length > 1) {
			foreach (string c in andConditions) {
				if (!CheckCondition(c)) return false;
			}
			return true;
		}
		// PARSE OR
		string[] orConditions = condition.Split('|');
		if (orConditions.Length > 1) {
			foreach (string c in orConditions) {
				if (CheckCondition(c)) return true;
			}
			return false;
		}
		// PARSE CONDITION
		bool not = condition.StartsWith("!");
		if (not) condition = condition.Substring(1);
		switch (condition) {
		case "grabbing" :
			return not ^ grabbing;
		case "test1" :
			return not ^ test1;
		default :
			return false; //TODO: Default true or default false?
		}
	}

	// Temporary test function 
	public void test() {
		Debug.Log ("----------");
		foreach (GameObject go in objectInFov) {
			Debug.Log (go);
		}
		botActions.DoAction ("move");
		//botActions.DoAction ("grab");
	}

	// Implementation of IBotBrain.NotifyAction
	public void NotifyAction(string action) {
		switch (action) {
		case "grab":
			grabbing = true;
			Debug.Log("Grab Completed");
			break;
		default :
			break;
		}
	}

	/*!
	 * An auxiliary function to print the local map in the Debug.Log
	 */
	public void printMap() {
		string res = "";
		for (int i=0;i<rsize;i++) {
			for (int j=0;j<csize;j++) {
				res += myMap [i * csize + j] + " ";
			}
			res += "\n";
		}
		Debug.Log(res);
	}
}
