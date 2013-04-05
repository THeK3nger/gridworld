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

	public bool raycastTest = true;

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
		GameObject obj = other.gameObject;	// Reference to the entering object.
		if (raycastTest) {
			GameObject bot = gameObject.transform.parent.gameObject; // Reference to the bot object.
			RaycastHit hit = new RaycastHit();
			Vector3 offset = new Vector3(0,1,0);
			Vector3 direction = (obj.transform.position - (bot.transform.position + offset)).normalized; // Direction between bot and other.
			Physics.Raycast(bot.transform.position + offset, direction, out hit);
			if (hit.transform.gameObject.Equals(obj)) { // If there are no objects between the bot and the entering object,
				parentControl.objectEnteringFOV(obj);
			}
		} else {
			parentControl.objectEnteringFOV(obj);
		}
    }

	/**
	 * Collistion callback fot exiting objects.
	 */
	void OnTriggerExit(Collider other) {
		GameObject obj = other.gameObject;
		parentControl.objectLeavingFOV(obj);
    }
	
}