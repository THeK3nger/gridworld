using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Implementation of IBotDeliberator that navigate the map using the 
 * hierarchical areas representation.
 *
 * At every step a random point in a random adiacent area is choosen.
 *
 * \author Davide Aversa
 * \version 1.0
 * \date 2013
 */
public class RandomAreaWalker : MonoBehaviour, IBotDeliberator {

	private BotControl control;				//A reference to the parent control.
	private GridWorldMap mapworld;			//A reference to the original map.
	private Queue<string> commandBuffer;

    private string walkable = ".X";

	// Use this for initialization
	void Start () {
		control = gameObject.GetComponent<BotControl>();
		mapworld = GameObject.Find("MapGenerator").GetComponent<GridWorldMap>();	
		commandBuffer = new Queue<string>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string GetNextAction() {
        Debug.Log("GetNextAction");
		if (commandBuffer.Count != 0) {
			return commandBuffer.Dequeue();
		}
        Debug.Log("Buffer empty! Search for new action.");
		Vector3 current = gameObject.transform.position;
		int[] currentGrid = mapworld.GetIndexesFromWorld(current.x,current.z);
		int currentArea = mapworld.GetArea(currentGrid[0],currentGrid[1]);
        Debug.Log("Current Area = " + currentArea);
		HashSet<int> connectedAreas = control.ConnectedAreas(currentArea);
        Debug.Log("Found " + connectedAreas.Count + " connected areas.");
		// Pick a random area.
        int[] areaArray = new int[connectedAreas.Count];
        connectedAreas.CopyTo(areaArray);
		int randomArea = areaArray[Random.Range(0,areaArray.Length)];
        Debug.Log("I choose " + randomArea);
		// Move to door and then to target position.
        int door = mapworld.GetDoorByAreas(currentArea, randomArea, currentGrid[0], currentGrid[1]);
        if (door == -1)
        {
            // If there are no open doors stay in the same area.
            commandBuffer.Enqueue(MoveToRandomAreaPoint(currentArea));
        }
        else
        {
            Debug.Log("Connected by " + door);
            // Else move to the door and then to the next area.
            commandBuffer.Enqueue(MoveToDoor(door));
            commandBuffer.Enqueue(MoveToRandomAreaPoint(randomArea));
        }
		return commandBuffer.Dequeue();
	}

    private string MoveToRandomAreaPoint(int area)
    {
        string result = "move ";
        char[] botMap = control.GetInternalMap();
        int chosenIdx = -1;
        int areaCount = 1;
        for (int idx = 0; idx < botMap.Length; idx++)
        {
            if (walkable.IndexOf(botMap[idx]) != -1 && mapworld.GetArea(idx) == area)
            {
                if (Random.Range(0, areaCount) == 0)
                {
                    chosenIdx = idx;
                }
                areaCount++;
            }
        }
        if (chosenIdx == -1) return "";
        int[] chosenIJ = mapworld.GetPositionFromArrayIndex(chosenIdx);
        float[] chosenXZ = mapworld.GetWorldFromIndexes(chosenIJ[0], chosenIJ[1]);
        return result + chosenXZ[0] + " " + chosenXZ[1];
    }

    private string MoveToDoor(int door)
    {
        string result = "move ";
        int[] doorIJ = mapworld.GetPositionFromArrayIndex(door);
        float[] doorXZ = mapworld.GetWorldFromIndexes(doorIJ[0], doorIJ[1]);
        result = result + doorXZ[0] + " " + doorXZ[1];
        return result;
    }
}
