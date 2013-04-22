using UnityEngine;
using System.Collections;

/**
 * Mouse click listener for object.
 * 
 * Add ClickListener to an Object to force bot to perform
 * a given action when the user click on it.
 */
public class ClickListener : MonoBehaviour {

	public string action = "move";			// The action that you want to perform on click.
	public string botName = "Bot(Clone)";	// The name of the target bot.

	GameObject bot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown() {
		bot = GameObject.Find(botName);
		float x = transform.position.x;
		float z = transform.position.z;
		BotActions botActions = bot.GetComponent<BotActions>();
		string command = action + " " + x + " " + z; 
		botActions.DoAction("stop");
	}
}
