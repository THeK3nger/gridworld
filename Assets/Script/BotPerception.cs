using UnityEngine;
using System.Collections;

/**
 * Implement a perception by collision system for the attached object.
 * 
 * \author Davide Aversa
 * \version 1.0
 * \date 2013
 * \pre This component must be attached to a *perception mesh* attached to the bot.
 * The bot must have a BotControl instance attached to itself.
 */
[RequireComponent (typeof (Collider))]
public class BotPerception : MonoBehaviour {

	private BotControl parentControl;	/**< A reference to the IBotControl instance attache to the bot. */

	// Use this for initialization
	void Awake () {
		parentControl = gameObject.transform.parent.gameObject.GetComponent<BotControl>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/**
	 * Collision callback for entering objects.
	 */
	void OnTriggerEnter(Collider other) {
		GameObject obj = other.gameObject;
		parentControl.objectEnteringFOV(obj);
		// TODO: Check for visibility test (is the object hidden by another object?); 
    }

	/**
	 * Collistion callback fot exiting objects.
	 */
	void OnTriggerExit(Collider other) {
		GameObject obj = other.gameObject;
		parentControl.objectLeavingFOV(obj);
    }
	
}