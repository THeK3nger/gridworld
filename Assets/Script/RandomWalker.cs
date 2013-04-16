using UnityEngine;
using System.Collections;

/**
 * Implementation of IBotDeliberator that navigate the map using a
 * random direction.
 *
 * At every step a random direction from the current point is choosen.
 *
 * \author Davide Aversa
 * \version 1.0
 * \date 2013
 */
public class RandomWalker : MonoBehaviour, IBotDeliberator {

	private BotControl control;				//A reference to the parent control.
	private GridWorldMap mapworld;			//A reference to the original map.

    public string interestType { get { return ""; } }

	// Use this for initialization
	void Start () {
		control = gameObject.GetComponent<BotControl>();
		mapworld = GameObject.Find("MapGenerator").GetComponent<GridWorldMap>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string GetNextAction() {
		// Return a random movement.
		int num = Random.Range(0,4);
		Vector3 current = gameObject.transform.position;
		float x = 0;
		float z = 0;
		switch (num) {
		case 0 : // Up
			x = current.x;
			z = current.z - mapworld.gridSize;
			break;
		case 1 : // Down
			x = current.x;
			z = current.z + mapworld.gridSize;
			break;
		case 2 : // Right
			x = current.x + mapworld.gridSize;
			z = current.z;
			break;
		case 3 : // Left
			x = current.x - mapworld.gridSize;
			z = current.z;
			break;
		}
		return "move " + x + " " + z;
	}
}
