using UnityEngine;
using System.Collections;

public class NetDeliberator : GridWorldBehaviour, IBotDeliberator {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public string GetNextAction()
    {
        throw new System.NotImplementedException();
    }

    public void NotifyObjectChange(GameObject obj, char type)
    {
        throw new System.NotImplementedException();
    }

    public string interestType
    {
        get { return ""; }
    }
}
