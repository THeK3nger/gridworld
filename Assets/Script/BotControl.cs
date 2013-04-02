using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * The main brain of a Bot.
 *
 * The Bot Controller class is the core of the Bot AI. It is the nexus between all the AI elements
 * like perception, action and planning/behavior components.
 * 
 * \author Davide Aversa
 * \version 1.0
 * \date 2013
 */
public class BotControl : MonoBehaviour {

	// CONTROL INSPECTOR PARAMETERS
	public float thinkTick = 1;

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
		// Run Thread Function Every `n` second
		InvokeRepeating("ThinkLoop", 3, thinkTick);
	}
	 
	// Update is called once per frame
	void Update () {
	 
	}

	/**
	 * Callback function called by the Perception component
	 * when an object enter in the collision FOV object.
	 * 
	 * \param obj The entering GameObject.
	 */
	public void objectEnteringFOV(GameObject obj) {
	 	// Extract Type and update the map.
		SmartObjects attributes = obj.GetComponent<SmartObjects> ();
		char type = attributes.type[0];
		int idx = mapworld.getArrayIndexFromWorld (obj.transform.position.x, obj.transform.position.z);
		objectInFov.Add (obj);
		myMap [idx] = type;		
	}

	/**
	 * Callback function called by the Perception component
	 * when an object leaves the collision FOV object.
	 * 
	 * \param obj The leaving GameObject.
	 */
	public void objectLeavingFOV(GameObject obj) {
		objectInFov.Remove (obj);
	}

	/**
	 * CheckCondition parse a condition formula and return a single boolean value.
	 *
	 * TODO: Define formula syntax.
	 * 
	 * \param condition The input condition.
	 * \return The thruth value for the condition formula.
	 */
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

	// TODO: ThinkLoop 
	public void ThinkLoop() {
		Debug.Log ("----------");
		foreach (GameObject go in objectInFov) {
			Debug.Log ((go.GetComponent<SmartObjects>()).type);
		}
		//botActions.DoAction ("move");
		//botActions.DoAction ("grab");
	}

	/**
	 * Used by BotAction to notify the controller about the success of the given action.
	 * 
	 * \param action The action notification string (TODO: to be defined).
	 */
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
