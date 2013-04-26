using UnityEngine;
using System.Collections;

using Pathfinding;

/**
 * This class is a collection of low-level actions that a bot can do.
 * 
 * These actions can be invoked by BotControl to perform high-level actions.
 * 
 * The avaiable actions are:
 * 	- `move x z` : Move the bot to the <x,0,z> world position.
 *  - `lookat x z` : Look at the <x,0,z> point.
 *  - `grab` : Grab an item in the current position.
 * 
 * TODO: Complete Action Specification
 * 
 * \author Davide Aversa
 * \version 1.0
 * \date 2013
 * \pre This class needs an instance of GridWorldMap and BotControl.
 */
public class BotActions : GridWorldBehaviour {

	public float moveBaseSpeed = 2; 		/**< Walk speed in m/s. */

	private bool actionComplete = true;		/**< True if the last action is completed. */
	private bool actionSuccess = true;		/**< True if the last action is completed successfully. */ 

	private BotControl parentControl;		/**< A reference to a BotControl instance. */
    private BotAttributes attributes;

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
		parentControl = gameObject.GetComponent<BotControl>();
        attributes = gameObject.GetComponent<BotAttributes>();
	}

	/**
	 * Perform the given action (if exists).
	 * 
	 * \param action The action that must be executed.
	 * \retval true If the action can be executed.
	 * \retval false If the action can not be executed.
	 */
    public bool DoAction(string action)
    {
        Debug.Log("Action Received: " + action);
        string[] command = action.Split(' ');
        if (action == "stop")
        {
            AbortCurrentAction();
            return true;
        }
        if (actionComplete)
        {
            actionComplete = false;
            actionSuccess = false;
            switch (command[0])
            {
                case "move":
                    MoveTo(float.Parse(command[1]), float.Parse(command[2]));
                    return true;
                case "grab":
                    Grab();
                    return true;
                default:
                    return false;
            }
        }
        return false;
    }

    public void AbortCurrentAction()
    {
        if (!actionComplete)
        {
            // If there are any animation running, stop it.
            iTween.Stop(gameObject);
            // Snap to the nearest grid point.
            Vector3 current = gameObject.transform.position;
            MoveTo(mapWorld.SnapCoord(current.x), mapWorld.SnapCoord(current.z));
        }
    }

	/*!
	 * Check if the last action is completed.
	 * 
	 * \retval true If the last action is completed.
	 * \retval false If the last action is still running.
	 */
	public bool LastActionComplete() {
		return actionComplete;
	}

	/**
	 * Check if the last action is completed succesfully.
	 * 
	 * \retval true If the last action is completed succesfully.
	 * \retval false If the last action is not completed or completed with faliure.
	 */
	public bool LastActionCompletedSuccessfully() {
		return actionComplete && actionSuccess;
	}

	/**
	 * Move the bot to the world plane <x,z> position.
	 * 
	 * \param x Desired x world location.
	 * \param z Desired z world location.
	 */
	void MoveTo(float x, float z) {
		Vector3 target = new Vector3 (x, 0, z);
		(gameObject.GetComponent("Seeker") as Seeker).StartPath(gameObject.transform.position, target,this.PathFoundCallback);
	}

	/**
	 * Calback called by Aron Pathfinding Algorithm when a path is available.
	 * 
	 * \param path The desired path.
	 */
	void PathFoundCallback(Path path) {
        float moveSpeed = moveBaseSpeed; // TODO: get slower with gold.
		animation.CrossFade("walk");
		Vector3[] array_path = path.vectorPath.ToArray ();
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
	void onMoveToPathComplete() {
		animation.CrossFade ("idle1");
		actionComplete = true;
		actionSuccess = true;
		//LookAt(new Vector3(0,0,0));
		parentControl.NotifyAction("move");
	}

	/**
	 * Look at the `dir` world point.
	 * 
	 * \param dir The point that we want to look at.
	 */
	void LookAt(Vector3 dir) {
		iTween.LookTo(gameObject,dir,1.0f);
	}

	/**
	 * Attack
	 */
	void Attack() {

	}

	/**
	 * Grab the object in current location.
	 */
    void Grab()
    {
        Vector3 current = gameObject.transform.position;
        char currentItem = mapWorld.GetMapElement(current.x, current.z);
        Debug.Log(current.x + " " + current.z + " " + currentItem);
        if (mapWorld.ElementIs("collectable", currentItem))
        {
            DestroyGameObjectByPosition(current, 'G');
            attributes.goldCarrying += 100;
            mapWorld.SetMapElement(current.x, current.z, '.');
            parentControl.NotifyAction("grab");
        }
        else
        {
            Debug.Log("Nothing to Grab!!!");
        }
        actionComplete = true;
        actionSuccess = true;
        // TODO: How to invoke a return value?
    }

	/**
	 * Release the object in current location.
	 */
	void Release() {

	}

    /**
     * Destroy an object of a given type in the given position.
     * 
     * \param position The desired object position.
     * \param type The desired object type.
     */
    private void DestroyGameObjectByPosition(Vector3 position, char type)
    {
        Debug.Log("Destroy!");
        Collider[] hitColliders = Physics.OverlapSphere(position, 1.5f);
        foreach (Collider c in hitColliders)
        {
            SmartObjects so = c.gameObject.GetComponent<SmartObjects>();
            if (so != null)
            {
                if (so.type[0] == type &&
                    System.Math.Abs(c.gameObject.transform.position.x - position.x) < 0.1 &&
                    System.Math.Abs(c.gameObject.transform.position.z - position.z) < 0.1)
                {
                    Debug.Log("Destroy: " + so.type[0]);
                    Destroy(c.gameObject);
                    return;
                }
            }
        }
    }
}
