using UnityEngine;
using System.Collections;

public class NetDeliberator : GridWorldBehaviour, IBotDeliberator {

    public bool averaging = false;
    public bool useAction = true;
    public float tileSize; // In mm
    public float xOffset;
    public float yOffset;

    private bool needNewPos;
    private float myX;
    private float myY;

    GameObject guiAlarm1;

	// Use this for initialization
	void Start () {
        guiAlarm1 = GameObject.Find("Receiving");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public string GetNextAction()
    {
        float xtmp = (myX-xOffset) / tileSize;
        float ytmp = (myY-yOffset) / tileSize;
        float xGrid = mapWorld.SnapCoord(xtmp);
        float yGrid = mapWorld.SnapCoord(ytmp);
        Debug.Log("Camera X: " + myX + " Camera Y: " + myY + "Grid X: " + xGrid + " Grid Y: " + yGrid);
        return "move " + xGrid + " " + yGrid;
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
        myX = x;
        myY = y;
        Redfy red = guiAlarm1.GetComponent<Redfy>();
        red.receivingTimeout = 0.5f;
    }
}
