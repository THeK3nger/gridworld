using UnityEngine;
using System.Collections;

using Pathfinding;

/*!
 * BotActions is a collection of low-level actions that a bot can do.
 * 
 * This actions can be invoked by an IBotBrain to perform high-level actions.
 * 
 * TODO: Complete Action Specification
 */
public class BotActions : MonoBehaviour {

	public float moveSpeed = 1;
	public string controllerName = "BotControlBase";

	private bool actionComplete = true;
	private bool actionSuccess = true;

	private IBotControl parentControl;

	// Use this for initialization
	void Start () {
		// TODO: There is a way to make this generic?
		parentControl = gameObject.GetComponent(controllerName) as IBotControl;
	}

	/*!
	 * Perform the given action (if exists).
	 * 
	 * @param action The action that must be executed.
	 */
	public bool DoAction(string action) {
		if (actionComplete) {
		switch (action) {
			case "move" :
				// TODO: Switch among actions.
				actionComplete = false;
				actionSuccess = false;
				MoveTo(8.5f,8.5f);			
				return true;
			case "grab" :
				actionComplete = false;
				actionSuccess = false;
				Grab();
				return true;
			default :
				return false;
			}
		}
		return false;
	}

	/*!
	 * Check if the last action is completed.
	 */
	public bool LastActionComplete() {
		return actionComplete;
	}

	/*!
	 * Check if the last action is completed succesfully.
	 */
	public bool LastActionCompletedSuccessfully() {
		return actionComplete && actionSuccess;
	}

	/*!
	 * Move the bot to the world plane <x,z> position.
	 */
	void MoveTo(float x, float z) {
		Vector3 target = new Vector3 (x, 0, z);
		(gameObject.GetComponent("Seeker") as Seeker).StartPath(gameObject.transform.position, target,this.PathFoundCallback);
	}

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

	void onMoveToPathComplete() {
		animation.CrossFade ("idle1");
		actionComplete = true;
		actionSuccess = true;
		LookAt(new Vector3(0,0,0));
	}

	/*!
	 * Face in 'dir' direction.
	 */
	void LookAt(Vector3 dir) {
		iTween.LookTo(gameObject,dir,1.0f);
	}

	/*!
	 * Attack
	 */
	void Attack() {

	}

	/*!
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

	/*!
	 * Release the object in current location.
	 */
	void Release() {

	}

	// TODO: Define more actions if needed!


}
