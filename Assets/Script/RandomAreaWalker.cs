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
		if (commandBuffer.Count != 0) {
			return commandBuffer.Dequeue();
		}
		Vector3 current = gameObject.transform.position;
		int[] currentGrid = mapworld.getIndexesFromWorld(current.x,current.z);
		int currentArea = mapworld.GetAreaFromPosition(currentGrid[0],currentGrid[1]);
		HashSet<int> connectedAreas = control.ConnectedAreas(currentArea);
		// Pick a random area.
        int[] areaArray = new int[connectedAreas.Count];
        connectedAreas.CopyTo(areaArray);
        Debug.Log(areaArray.Length);
		int randomArea = areaArray[Random.Range(0,areaArray.Length-1)];
		// Move to door and then to target position.
        int door = mapworld.GetDoorByAreas(currentArea, randomArea, currentGrid[0], currentGrid[1]);
        if (door == -1)
        {
            // If there are no open doors stay in the same area.
            commandBuffer.Enqueue(MoveToRandomAreaPoint(currentArea));
        }
        else
        {
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
            if (botMap[idx] == '.' && mapworld.GetAreaFromPosition(idx) == area)
            {
                if (Random.Range(0, areaCount-1) == 0)
                {
                    chosenIdx = idx;
                }
                areaCount++;
            }
        }
        if (chosenIdx == -1) return "";
        int[] chosenIJ = mapworld.GetPositionFromArrayIndex(chosenIdx);
        float[] chosenXZ = mapworld.getWorldFromIndexes(chosenIJ[0], chosenIJ[1]);
        return result + chosenXZ[0] + " " + chosenXZ[1];
    }

    private string MoveToDoor(int door)
    {
        string result = "move ";
        int[] doorIJ = mapworld.GetPositionFromArrayIndex(door);
        float[] doorXZ = mapworld.getWorldFromIndexes(doorIJ[0], doorIJ[1]);
        result = result + doorXZ[0] + " " + doorXZ[1];
        return result;
    }
}
