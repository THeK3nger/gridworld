using UnityEngine;
using System.Collections;

public class RandomWalker : MonoBehaviour, IBotDeliberator {

	private BotControl control;

	// Use this for initialization
	void Start () {
		control = gameObject.GetComponent<BotControl>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string GetNextAction() {
		int num = Random.Range(0,4);
		Vector3 current = gameObject.transform.position;
		float x = 0;
		float z = 0;
		switch (num) {
		case 0 : // Up
			x = current.x;
			z = current.z - 1;
			break;
		case 1 : // Down
			x = current.x;
			z = current.z + 1;
			break;
		case 2 : // Right
			x = current.x + 1;
			z = current.z;
			break;
		case 3 : // Left
			x = current.x - 1;
			z = current.z;
			break;
		}
		return "move " + x + " " + z;
	}
}
