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
public class RandomAreaWalker : GridWorldBehaviour, IBotDeliberator {

	private BotControl control;				    //A reference to the parent control.
    private Dictionary<int, bool> doorsState;   // Contains the doors status (open or closed).
    private int[] mapsize;

	private Queue<string> commandBuffer;

    public string interestType { get { return "D"; } }

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
		control = gameObject.GetComponent<BotControl>();
        doorsState = new Dictionary<int, bool>();
		commandBuffer = new Queue<string>();
        mapsize = mapWorld.GetMapSize();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string GetNextAction() {
		if (commandBuffer.Count != 0) {
			return commandBuffer.Dequeue();
		}
        Debug.Log("Buffer empty! Search for new action.");
		Vector3 current = gameObject.transform.position;
		int[] currentGrid = mapWorld.GetIndexesFromWorld(current.x,current.z);
		int currentArea = mapWorld.GetArea(currentGrid[0],currentGrid[1]);
        Debug.Log("Current Area = " + currentArea);
		HashSet<int> connectedAreas = ConnectedAreas(currentArea);
        Debug.Log("Found " + connectedAreas.Count + " connected areas.");
		// Pick a random area.
        int[] areaArray = new int[connectedAreas.Count];
        connectedAreas.CopyTo(areaArray);
		int randomArea = areaArray[Random.Range(0,areaArray.Length)];
        Debug.Log("I choose " + randomArea);
		// Move to door and then to target position.
        int door = mapWorld.GetDoorByAreas(currentArea, randomArea, currentGrid[0], currentGrid[1]);
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

    public void NotifyObjectChange(GameObject obj, char type)
    {
        Debug.Log("Deliberator Notified!");
        Door door = obj.GetComponent<Door>();
        Vector3 doorPos = obj.transform.position;
        int idx = mapWorld.GetArrayIndex(doorPos.x, doorPos.z);
        doorsState[idx] = door.isOpen;
    }

    private string MoveToRandomAreaPoint(int area)
    {
        string result = "move ";
        int chosenIdx = -1;
        int areaCount = 1;
        int totalSize = mapsize[0]*mapsize[1];
        for (int idx = 0; idx < totalSize; idx++)
        {
            if (mapWorld.GetArea(idx) == area)
            {
                if (Random.Range(0, areaCount) == 0)
                {
                    chosenIdx = idx;
                }
                areaCount++;
            }
        }
        if (chosenIdx == -1) return "";
        int[] chosenIJ = mapWorld.GetPositionFromArrayIndex(chosenIdx);
        float[] chosenXZ = mapWorld.GetWorldFromIndexes(chosenIJ[0], chosenIJ[1]);
        return result + chosenXZ[0] + " " + chosenXZ[1];
    }

    private string MoveToDoor(int door)
    {
        string result = "move ";
        int[] doorIJ = mapWorld.GetPositionFromArrayIndex(door);
        float[] doorXZ = mapWorld.GetWorldFromIndexes(doorIJ[0], doorIJ[1]);
        result = result + doorXZ[0] + " " + doorXZ[1];
        return result;
    }

    /**
     * Return a list of the open doors (according to the bot internal knowledge).
     *
     * \return The list of open doors.
     */
    private List<int> GetOpenDoors()
    {
        List<int> result = new List<int>();
        foreach (KeyValuePair<int, bool> entry in doorsState)
        {
            if (doorsState[entry.Key])
            {
                result.Add(entry.Key);
            }
        }
        return result;
    }

    /**
     * Return the connected areas (acording to the bot internal knowledge)
     * to the given area label.
     *
     * \param area The input area.
     * \return The list of the connected area.
     */
    private HashSet<int> ConnectedAreas(int area)
    {
        List<int> open_doors = GetOpenDoors();
        List<int> result = new List<int>();
        result.Add(area); // An area is always connected to itself.
        foreach (int door in open_doors)
        {
            List<int> doorAreas = mapWorld.GetAreasByDoor(door);
            if (doorAreas.IndexOf(area) != -1)
                result.AddRange(doorAreas);
        }
        return new HashSet<int>(result);
    }


}
