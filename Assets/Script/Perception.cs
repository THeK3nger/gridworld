using UnityEngine;
using System.Collections;

public class Perception : MonoBehaviour {
	
	private int[] myMap;			//Store local map perception.
	private int rsize, csize;		//Map size.
	private GridWorldMap mapworld;	//A reference to the original map.


	// Use this for initialization
	void Start () {
		mapworld = GameObject.Find("MapGenerator").GetComponent<GridWorldMap>();
		int[] sizes = mapworld.getMapSize ();
		rsize = sizes [0];
		csize = sizes [1];
		myMap = new int[rsize * csize];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*!
	 * OnTriggerEnter the bot update its world representation with the new data
	 * coming from the sensors.
	 */
	void OnTriggerEnter(Collider other) {
		if (mapworld == null) return;
		int[] idxs = mapworld.getIndexFromWorld (other.transform.position.x, other.transform.position.z);
		int itemid = mapworld.getMapElement (idxs[0],idxs[1]);
		myMap [idxs [0] * csize + idxs [1]] = itemid;
		//printMap ();
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