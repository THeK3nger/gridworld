using UnityEngine;
using System.Collections;

public interface IBotControl {

	/*!
	 * objectEntringFOV is a callback function called by the Perception component
	 * when an object enter in the collision FOV object.
	 * 
	 * @param obj The entering GameObject.
	 */
	void objectEnteringFOV(GameObject obj);

	/*!
	 * objectEnteringFOV is a callback function called by the Perception component
	 * when an object leaves the collision FOV object.
	 * 
	 * @param obj The leaving GameObject.
	 */
	void objectLeavingFOV(GameObject obj);

	/*!
	 * botDoAction invokes an action on the Bot
	 */
	void botDoAction(string action);

}
