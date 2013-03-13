using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
public class Perception : MonoBehaviour {

	// Use this for initialization
	void Start () {

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
		transform.parent.gameObject.SendMessage ("objectEnteringFOV", obj);
    }

	void OnTriggerExit(Collider other) {
		GameObject obj = other.gameObject;
		transform.parent.gameObject.SendMessage ("objectLeavingFOV", obj);
    }
	
}