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
	 * CheckCondition parse a condition formula and return a single boolean value.
	 *
	 * TODO: Define formula syntax.
	 * 
	 * @param condition The input condition.
	 * @return The thruth value for the condition formula.
	 */
	bool CheckCondition(string condition);

	/*!
	 * Used by BotAction to notify the controller about the success of the given action.
	 * 
	 * @param action The action notification string (TODO: to be defined).
	 */
	void NotifyAction(string action);

}
