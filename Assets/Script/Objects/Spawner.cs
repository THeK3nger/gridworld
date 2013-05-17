using UnityEngine;
using System.Collections;

public class Spawner : GridWorldBehaviour {

    public GameObject obj;
    public float spawnTime;
    public int maxItems;
    public Vector3 rotation;
    public string itemChar;

    private int itemSpawned;

	// Use this for initialization
    protected override void Awake()
    {
        base.Awake();
    }

	void Start () {
        InvokeRepeating("SpawnLoop", 3, spawnTime);
	}

    public void SpawnLoop()
    {
        if (itemSpawned < maxItems)
        {
            int randomArea = mapWorld.SelectRandomArea();
            int randomIdx = mapWorld.SelectRandomAreaPosition(randomArea);
            if (mapWorld.GetMapElement(randomIdx) == 'G') return;
            int[] idx = mapWorld.GetPositionFromArrayIndex(randomIdx);
            float[] idxWorld = mapWorld.GetWorldFromIndexes(idx[0], idx[1]);
            Instantiate(obj, new Vector3(idxWorld[0], 0.5f, idxWorld[1]), Quaternion.Euler(rotation));
            mapWorld.SetMapElement(randomIdx, itemChar[0]); // Set gold in the global map. 
            itemSpawned++;
        }
    }

	// Update is called once per frame
	void Update () {
	
	}

    public void KillItem()
    {
        itemSpawned--;
    }
}
