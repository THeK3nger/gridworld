using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
public class BotPerception : MonoBehaviour {

	public string controllerName = "BotControlBase";

	private IBotControl parentControl;

	// Use this for initialization
	void Awake () {
		parentControl = gameObject.transform.parent.gameObject.GetComponent(controllerName) as IBotControl;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*!
	 * OnTriggerEnter the bot update its world representation with the new data
	 * coming from the sensors.
	 */
	void OnTriggerEnter(Collider other) {
		GameObject obj = other.gameObject;
		parentControl.objectEnteringFOV(obj);
    }

	void OnTriggerExit(Collider other) {
		GameObject obj = other.gameObject;
		parentControl.objectLeavingFOV(obj);
    }
	
}