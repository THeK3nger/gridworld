using UnityEngine;
using System.Collections;

public class Redfy : MonoBehaviour {

    public float receivingTimeout = 0.5f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        receivingTimeout -= Time.deltaTime;
        if (receivingTimeout <= 0)
        {
            guiTexture.color = Color.red;
        }
        else
        {
            guiTexture.color = Color.green;
        }
	}

}
