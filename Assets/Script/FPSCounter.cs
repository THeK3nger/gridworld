using UnityEngine;
using System.Collections;
using System;

public class FPSCounter : MonoBehaviour {

    int count = 0;
    double totalTime = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        count++;
        totalTime = +Time.deltaTime;
        if (count % 60 == 0)
        {
            count = 0;
            gameObject.guiText.text = Math.Round(1.0 / totalTime) + "fps";
            totalTime = 0;
        }
	}
}
