using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

/**
 * This class is a collection of low-level actions that a bot can do.
 * 
 *  * The avaiable actions are:
 * 	- `move x z` : Move the bot to the <x,0,z> world position.
 *  - `lookat x z` : Look at the <x,0,z> point.
 *  - `grab` : Grab an item in the current position.
 *  - `drop` : Unload the gold in the area (if the area is valid).
 */
[RequireComponent(typeof(BotActions))]
public class RoombaAction : GridWorldBehaviour
{

    private BotActions parentAction;

    public float moveBaseSpeed = 2; 		/**< Walk speed in m/s. */
    public float speedDecreaseRate;         /**< Speed decrease rate by gold. */
	public int batteryLevel = 100;
	public string TAG;
	
	private Dictionary<string,int[]> rooms;
	public string currentRoom;

	// Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        parentAction = gameObject.GetComponent<BotActions>();
	}

    void Start()
    {
		batteryLevel = 100;
		rooms = new Dictionary<string, int[]>();
		rooms.Add("A",new int[]{7,3});
		rooms.Add("B",new int[]{16,3});
		rooms.Add("C",new int[]{21,7});
		rooms.Add("D",new int[]{16,12});
		rooms.Add("E",new int[]{7,12});
		rooms.Add("F",new int[]{3,7});
        parentAction.RegisterAbortAction(AbortCurrentAction);
        parentAction.RegisterNewAction("move", MoveTo);
        parentAction.RegisterNewAction("snapshot", Snapshot);
        parentAction.RegisterNewAction("clean", Clean);
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void AbortCurrentAction()
    {
        if (!parentAction.LastActionComplete())
        {
            // If there are any animation running, stop it.
            iTween.Stop(gameObject);
            // Snap to the nearest grid point.
            Vector3 current = gameObject.transform.position;
            MoveTo(mapWorld.SnapCoord(current.x), mapWorld.SnapCoord(current.z));
        }
    }

    /**
     * Move the bot to the world plane <x,z> position.
     * 
     * \param x Desired x world location.
     * \param z Desired z world location.
     */
    void MoveTo(float x, float z)
    {
        Vector3 target = new Vector3(x, 0, z);
        (gameObject.GetComponent("Seeker") as Seeker).StartPath(gameObject.transform.position, target, this.PathFoundCallback);
    }

    /**
     * Move the bot to the world plane <x,z> position.
     * 
     * \param moveCommand the parsed move command.
     */
    void MoveTo(string[] moveCommand)
    {
		if (batteryLevel<=0) {
			Debug.Log("BATTERY TOO LOW");
			return;
		}
        string new_room = moveCommand[1];
		bool start_left = currentRoom == "A" || currentRoom == "F" || currentRoom == "E";
		bool end_left = new_room == "A" || new_room == "F" || new_room == "E";
		if ((start_left && end_left) || (!start_left && !end_left)) {
			batteryLevel--;	
		} else {
			batteryLevel=100;
		}
		float[] dest = mapWorld.GetWorldFromIndexes(rooms[new_room][1],rooms[new_room][0]);
		currentRoom = new_room;
        MoveTo(dest[0], dest[1]);
    }

    /**
     * Calback called by Aron Pathfinding Algorithm when a path is available.
     * 
     * \param path The desired path.
     */
    void PathFoundCallback(Path path)
    {
        float moveSpeed = moveBaseSpeed;
        moveSpeed = System.Math.Max(0.5f, moveSpeed);
        //animation.CrossFade("walk");
        Vector3[] array_path = path.vectorPath.ToArray();
        iTween.MoveTo(gameObject, iTween.Hash
                      (
            "path", array_path,
            "orienttopath", true,
            "looktime", 1.0,
            "lookahead", 0.05,
            "axis", "y",
            "y", 1,
            "easetype", iTween.EaseType.linear,
            "time", iTween.PathLength(array_path) / moveSpeed,
            "oncomplete", "onMoveToPathComplete"
            ));
    }

    /**
     * Calback called by iTween when the path execution is completed.
     */
    void onMoveToPathComplete()
    {
        //animation.CrossFade("idle1");
        parentAction.NotifyActionComplete();
        parentAction.NotifyActionSuccess();
        parentAction.NotifyAction("move");
    }

    /**
     * Look at the `dir` world point.
     * 
     * \param dir The point that we want to look at.
     */
    void LookAt(Vector3 dir)
    {
        iTween.LookTo(gameObject, dir, 1.0f);
    }


	public void Snapshot(string[] param) {
		ParticleSystem psys = gameObject.GetComponentInChildren<ParticleSystem>();
		psys.Emit(100);
	}
	
	public void Clean(string[] param) {
			
	}
	
	public string GetState() {
		string batteryState = "HIGH";
		if (batteryLevel<50) batteryState = "LOW"; 
		return TAG + " " + currentRoom + " " + batteryState;
	}
}
