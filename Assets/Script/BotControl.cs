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
	public float thinkTick = 1;				//Time interval between a think cicle.
	public string deliberatorName;			//Name of the IBotDeliberator implementation.
	public bool deliberatorOn;				//True if deliberator is ON. 

	private char[] myMap;					//Store local map perception.
	private int rsize, csize;				//Map size.
	private GridWorldMap mapworld;			//A reference to the original map.
	private BotActions botActions;  		//Reference to the BotAction component.
	private IBotDeliberator deliberator;	//Reference to a IBotDeliberator interface.
	
	private List<GameObject> objectInFov; 	// Contains the list of object in the FOV.
	private Dictionary<int,bool> doorsState;// Contains the doors status (open or closed).

	// CONDITIONS TODO: to be defined
	private bool grabbing = false;
	private bool test1 = true;

	// STATE
	private enum Status { IDLE, EXECUTING };
	private Status controlStatus;			// Controller Status.

	// Use this for initialization
	void Awake() {
		controlStatus = Status.IDLE;
		mapworld = GameObject.Find("MapGenerator").GetComponent<GridWorldMap>();
		int[] sizes = mapworld.getMapSize ();
		doorsState = new Dictionary<int,bool>();
		rsize = sizes [0];
		csize = sizes [1];
		myMap = new char[rsize * csize];
		objectInFov = new List<GameObject>();
		botActions = gameObject.GetComponent<BotActions> ();
		deliberator = gameObject.GetComponent(deliberatorName) as IBotDeliberator;
        // Update current position in myMap
        Vector3 current = gameObject.transform.position;
        int idx = mapworld.getArrayIndexFromWorld(current.x, current.z);
        myMap[idx] = mapworld.getMapElement(current.x, current.z);
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
		if (type=='D') {
			Door d = obj.GetComponent<Door> ();
			doorsState[idx] = d.isOpen;
		}
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
		if (controlStatus == Status.IDLE && deliberatorOn) {
			string nextaction = deliberator.GetNextAction();
			Debug.Log("Get " + nextaction);
			controlStatus = Status.EXECUTING;
			botActions.DoAction(nextaction);
		}
		//Debug.Log(objectInFov.Count);
		//botActions.DoAction ("move");
		//botActions.DoAction ("grab");
	}

	/**
	 * Used by BotAction to notify the controller about the success of the given action.
	 * 
	 * \param action The action notification string (TODO: to be defined).
	 */
	public void NotifyAction(string action) {
		controlStatus = Status.IDLE;
		switch (action) {
		case "grab":
			grabbing = true;
			Debug.Log("Grab Completed");
			break;
		default :
			break;
		}
	}

	/**
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

	/**
	 * Return the connected areas (acording to the bot internal knowledge)
	 * to the given area label.
	 *
	 * \param area The input area.
	 * \return The list of the connected area.
	 */
	public HashSet<int> ConnectedAreas(int area) {
		List<int> open_doors = GetOpenDoors();
		List<int> result = new List<int>();
        result.Add(area); // An area is always connected to itself.
		foreach (int door in open_doors) {
			List<int> doorAreas = mapworld.GetAreasByDoor(door);
			if (doorAreas.IndexOf(area) != -1)
				result.AddRange(doorAreas);
		}
		return new HashSet<int>(result);
	}

	/**
	 * Return a list of the open doors (according to the bot internal knowledge).
	 *
	 * \return The list of open doors.
	 */
	public List<int> GetOpenDoors() {
		List<int> result = new List<int>();
		foreach (KeyValuePair<int,bool> entry in doorsState) {
			if (doorsState[entry.Key]) {
				result.Add(entry.Key);
			}
		}
		return result;
	}

    public char[] GetInternalMap()
    {
        return myMap; // TODO: Check if is better (and efficient) to return a copy
    }

}
