using UnityEngine;
using System.Collections;
using System;
using Pathfinding;

public class GridWorldMap : MonoBehaviour {
	
	// MapFile to load.
	public TextAsset mapFile;
	public float gridSize;
	public GameObject wall;
	public GameObject floor;
	public GameObject bot;

	private GameObject astar;
	private int[] staticMap;
	private int rsize;
	private int csize;

	// Use this for initialization
	void Start () {
		BuildMap();
		CreatePathfindingGrid ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void CreatePathfindingGrid() {
		astar = GameObject.Find ("A*");
		AstarPath astarpath = astar.GetComponent<AstarPath> ();
		GridGraph gridgraph = astarpath.astarData.AddGraph (typeof(GridGraph)) as GridGraph;
		// LayerMask
		int obstacles = 1 << LayerMask.NameToLayer ("Obstacles");
		int walkable = 1 << LayerMask.NameToLayer ("Walkable");
		// GridGraph Configuration
		gridgraph.width = csize;
		gridgraph.depth = rsize;
		gridgraph.nodeSize = gridSize;
		gridgraph.UpdateSizeFromWidthDepth ();
		gridgraph.center = new Vector3 (rsize * gridSize / 2.0f, -0.2f, csize * gridSize / 2.0f);
		gridgraph.collision.mask = obstacles;
		gridgraph.collision.diameter = 0.8f;
		gridgraph.collision.height = 1f;
		gridgraph.collision.heightMask = walkable;
		astarpath.Scan ();
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
	 * LEGEND (provisory)
	 * 	- 0 : Void
	 *  - 1 : Wall
	 *  - 100 : Floor
	 *  - 201 : Bot
	 * 
	 * TODO: Add more map items
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
	 * 
	 * \param x World x coordinate
	 * \param z World z coordinate
	 * \return A size two array with the <i,j> indexes.
	 */
	public int[] getIndexesFromWorld(float x, float z) {
		int i = (int) (x/gridSize - 0.5);
		int j = (int) (z/gridSize - 0.5);
		int[] res = {i,j};
		return res;
	}

	/*!
	 * Compute matrix indexes from world position.
	 * 
	 * \param x World x coordinate
	 * \param z World z coordinate
	 * \return Index of the linearized map array.
	 */
	public int getArrayIndexFromWorld(float x, float z) {
		int i = (int) (x/gridSize - 0.5);
		int j = (int) (z/gridSize - 0.5);
		return i * csize + j;
    }

	/*!
	 * Get the map element in the current world position.
	 * 
	 * \param x World x coordinate.
	 * \param z World z coordinate.
	 */
	public int getMapElement(float x, float z) {
		int idx = getArrayIndexFromWorld (x, z);
		return staticMap[idx];
	}

	/*!
	 * Convert a pair of matrix indexes <i,j> in the corresponding
	 * index of the linearized array associated to the map matrix.
	 * 
	 * \param i The row index.
	 * \param j The column index.
	 * \return The associated linearized array index.
	 */
	public int getArrayIndex(int i, int j) {
		return i * csize + j;
	}

	/*!
	 * Convert a pair of matrix indexes <i,j> in the corresponding
	 * index of the linearized array associated to the map matrix.
	 * 
	 * \param idxs A pair <i,j> of indexes.
	 * \return The associated linearized array index.
	 */
    public int getArrayIndex(int[] idxs) {
		return idxs [0] * csize + idxs [1];
	}

	/*!
	 * Return the map size.
	 * 
	 * \return A pair <rsize,csize> where rsize is the number of rows in the matrix and
	 * csize is the number of columns.
	 */
	public int[] getMapSize() {
		int[] res = {this.rsize,this.csize};
		return res;
	}
}
