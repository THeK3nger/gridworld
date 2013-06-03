using UnityEngine;
using System.Collections;

public class SimpleCondition : GridWorldBehaviour, IBotDeliberator
{

    private BotControl control;
    private RandomWalker randomWalker;

    void Awake()
    {
        base.Awake();
        control = gameObject.GetComponent<BotControl>();
        randomWalker = gameObject.GetComponent<RandomWalker>();
        InvokeRepeating("PrintGoldLocation", 0, 3);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public string GetNextAction()
    {
        return randomWalker.GetNextAction();
    }

    public void NotifyObjectChange(GameObject obj, char type)
    {
        Debug.Log("+++++++++++" + type);
        if (type == 'G')
        {
            Vector3 position = obj.transform.position;
            int[] gPos = mapWorld.GetIndexesFromWorld(position.x, position.z);
            control.internalKnowledge["location:" + gPos[0] + " " + gPos[1] + " gold"] = true;
        }
    }

    public string interestType
    {
        get { return "G"; }
    }

    public void PrintGoldLocation()
    {
        foreach (string[] ss in control.internalKnowledge.GetEnumerator("location", "$1 $2 gold"))
        {
            Debug.Log("GOLD: " + ss[0] + " " + ss[1]);
        }
    }
}
