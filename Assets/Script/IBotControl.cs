using UnityEngine;
using System.Collections;

/**
 * Basic interface for a Bot Controller class.
 * 
 * The Bot Controller class is the core of the Bot AI. It is the nexus between all the AI elements
 * like perception, action and planning/behavior components.
 * 
 * \author Davide Aversa
 * \version 1.0
 * \date 2013
 */
public interface IBotControl {

	/**
	 * Callback function called by the Perception component
	 * when an object enter in the collision FOV object.
	 * 
	 * \param obj The entering GameObject.
	 */
	void objectEnteringFOV(GameObject obj);

	/**
	 * Callback function called by the Perception component
	 * when an object leaves the collision FOV object.
	 * 
	 * \param obj The leaving GameObject.
	 */
	void objectLeavingFOV(GameObject obj);

	/**
	 * CheckCondition parse a condition formula and return a single boolean value.
	 *
	 * TODO: Define formula syntax.
	 * 
	 * \param condition The input condition.
	 * \return The thruth value for the condition formula.
	 */
	bool CheckCondition(string condition);

	/**
	 * Used by BotAction to notify the controller about the success of the given action.
	 * 
	 * \param action The action notification string (TODO: to be defined).
	 */
	void NotifyAction(string action);

}
