using UnityEngine;
using System.Collections;

public class BotAttributes : GridWorldBehaviour {

    public int goldCarrying { get; set; }
    public int goldStored { get; set; }
    public bool speedBoost;
    public int keys { get; set; }
    public int spawnArea { get; set; }
    public int life { get; set; }

    public float lavaDrainRate;

    public AudioClip lavasound;
    public AudioClip playerDeath;

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
        Vector3 current = transform.position;
        spawnArea = mapWorld.GetArea(current.x, current.z);
        life = 100;
        InvokeRepeating("CheckLava", 3, lavaDrainRate);
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("GOLD CARRYING: " + goldCarrying);
	}

    void CheckLava()
    {
        float x = transform.position.x;
        float z = transform.position.z;
        if (mapWorld.GetMapElement(x,z) == 'L')
        {
            life--;
            AudioSource.PlayClipAtPoint(lavasound, transform.position);
        }
        if (life <= 0)
        {
            Destroy(gameObject);
            GameObject gameover = GameObject.Find("GAMEOVER");
            gameover.guiText.material.color = Color.white;
            gameover.guiText.text = "GAME OVER";
            AudioSource.PlayClipAtPoint(playerDeath, transform.position);
        }
    }
}
