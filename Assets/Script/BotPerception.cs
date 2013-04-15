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
            if (RayCastVisibility(obj,bot)) {
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

    private bool RayCastVisibility(GameObject obj, GameObject bot)
    {
        RaycastHit hit = new RaycastHit();
        Vector3 offset = new Vector3(0, 1, 0);
        // Direction between obj and other.
        Vector3 direction = (obj.transform.position - (bot.transform.position + offset)).normalized;
        Physics.Raycast(bot.transform.position + offset, direction, out hit);
        return hit.transform.gameObject.Equals(obj);
    }
	
}
