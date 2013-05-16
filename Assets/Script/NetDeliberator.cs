using UnityEngine;
using System.Collections;

public class NetDeliberator : GridWorldBehaviour, IBotDeliberator {

    public bool averaging = false;
    public bool useAction = true;

    private bool needNewPos;
    private float myX;
    private float myY;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public string GetNextAction()
    {
        needNewPos = true;
        // TODO: Do some tricks on myX and myY
        return "move " + myX + " " + myY;
    }

    public void NotifyObjectChange(GameObject obj, char type)
    {
        return;
    }

    public string interestType
    {
        get { return ""; }
    }

    public void newPosition(float x, float y)
    {
        if (averaging || !needNewPos)
        {
            myX = (myX + x) * 0.5f;
            myY = (myY + y) * 0.5f;
        }
        else
        {
            myX = x;
            myY = y;
            if (needNewPos) needNewPos = false;
        }
    }
}
