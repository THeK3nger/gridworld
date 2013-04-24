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

	public float moveSpeed = 1; 			/**< Walk speed in m/s. */

	private bool actionComplete = true;		/**< True if the last action is completed. */
	private bool actionSuccess = true;		/**< True if the last action is completed successfully. */ 

	private BotControl parentControl;		/**< A reference to a BotControl instance. */

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
		parentControl = gameObject.GetComponent<BotControl>();
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
            Debug.Log(command[0]);
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
            Debug.Log(mapWorld);
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
	void Grab() {
		Debug.Log("GRAB ACTION");
		if (parentControl.CheckCondition("!grabbing")) {
			parentControl.NotifyAction("grab");
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

	// TODO: Define more actions if needed!
}
