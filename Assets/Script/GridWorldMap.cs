using UnityEngine;
using System.Collections;
using System;

public class GridWorldMap : MonoBehaviour {
	
	// MapFile to load.
	public TextAsset mapFile;
	public float gridSize;
	public GameObject wall;
	public GameObject floor;
	public GameObject bot;
	
	private int[] staticMap;
	private int rsize;
	private int csize;

	// Use this for initialization
	void Start () {
		BuildMap();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*!
	 * LoadMapFile loads the map definition from file.
	 */
	private void LoadMapFromFile() {
		// Load text file from TextAsset.
		string map_string = mapFile.text;
		string[] lines = map_string.Split('\n');
		// Parse map size. MapSize is in the first line of a map file.
		string[] tmp = lines[0].Split('x');
		rsize = int.Parse(tmp[0]);
		csize = int.Parse(tmp[1]);
		// Initialize map array
		staticMap = new int[rsize*csize];
		// Fill the map 
		int i = 0;
		for (int line=1;line<lines.Length;line++) {
			string[] map_items = lines[line].Split(',');
			for (int j=0;j<map_items.Length;j++) {
				staticMap[i] = int.Parse(map_items[j]);
				i++;
			}
		}
		if (i<rsize*csize-1) Debug.Log("ERROR: Invalid Map!");
	}

	/*!
	 * Build map from the internal representation.
	 *
	 * TODO: Add more map item
	 */
	private void BuildMap() {
		LoadMapFromFile();
		for (int i=0;i<rsize;i++) {
			for (int j=0;j<csize;j++) {
				int map_element = staticMap[i*csize+j];
				float x = gridSize*(i+0.5f); 
				float z = gridSize*(j+0.5f);
				if (map_element!=0){
					if (map_element==1)
						Instantiate(wall,new Vector3(x,1,z),Quaternion.identity);
					if (map_element>=100)
						Instantiate(floor,new Vector3(x,0.05f,z),Quaternion.identity);
					if (map_element==201)
						Instantiate(bot, new Vector3(x,0.1f,z),Quaternion.Euler(Vector3.up * 90));
				}
			}
		}	
	}

	/*!
	 * Compute matrix indexes from world position.
	 */
	public int[] getIndexFromWorld(float x, float z) {
		int i = (int) (x/gridSize - 0.5);
		int j = (int) (z/gridSize - 0.5);
		int[] res = {i,j};
		return res;
	}

	/*!
	 * Get the map element in the current world position.
	 */
	public int getMapElement(float x, float z) {
		int[] idx = getIndexFromWorld (x, z);
		return staticMap[idx[0]*csize+idx[1]];
	}

	/*!
	 * Return the map size.
	 */
	public int[] getMapSize() {
		int[] res = {this.rsize,this.csize};
		return res;
	}
}
