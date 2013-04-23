using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Pathfinding;

/**
 * Loads and stores information about the world map.
 * 
 * This class contains the real representation of the world and includes
 * a set of utility functions to convert world point into the <i,j> matrix
 * representations.
 * 
 * \author Davide Aversa
 * \version 1.0
 * \date 2013
 */
public class GridWorldMap : MonoBehaviour
{

    public TextAsset mapFile; 	/**< Map file to load. */
	public float gridSize; 		/**< The base size of the grid. */
	public GameObject wall;		/**< The object prefab used as "wall". */
	public GameObject floor;	/**< The object prebab used as "floor". */
	public GameObject bot;		/**< The object prefab used as "bot". */
	public GameObject door;		/**< The object prefab used as "door". */

	private GameObject astar;	/**< A reference to the A* object (for pathfinding). */
	private char[] staticMap;	/**< The global representation of the world map. */
	private int[] areasMap;		/**< Areas representation of the world map. */
	Dictionary<int,List<int>> doors; /**< List of doors between areas. */
	private int rsize;			/**< Number of rows. **/
	private int csize;			/**< Number of columns. **/

	// Use this for initialization
	void Start () {
		BuildMap();
		CreatePathfindingGrid();
	}

	/**
	 * Initialize the pathfinding GridGrpah on the GridWorld.
     * 
     * This method build the a grid graph on the full map using the Aron
     * Pathfinding extension.
	 */
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

	/**
	 * Loads the map definition from the linked file.
	 */
	private void LoadMapFromFile() {
		// Load text file from TextAsset.
		string map_string = mapFile.text;
		string[] lines = map_string.Split('\n');
		// Parse map size.
		int lidx = 0; // Parsing line index
		string current = "";
		while (current != "map") {
			current = lines[lidx];
			string[] par = current.Split(' '); // Map Parameters, Ignore "type".
			switch (par[0]) {
			case "height" :
				rsize = int.Parse(par[1]);
				break;
			case "width" :
				csize = int.Parse(par[1]);
				break;
			default :
				break;
			}
			lidx++;
		}
		if (rsize == 0 || csize == 0) {
            throw new Exception("Invalid Map File!");
		}
		// Initialize map array
		staticMap = new char[rsize*csize];
		// Fill the map 
		int i = 0;
		for (int line=lidx;line<lines.Length;line++) {
			string map_items = lines[line];
			for (int j=0;j<map_items.Length;j++) {
				staticMap[i] = map_items[j];
				i++;
			}
		}
        if (i < rsize * csize - 1) throw new Exception("Invalid Map File!");

		// Find Areas
		AreaFinder af = new AreaFinder(staticMap,rsize,csize);
		this.areasMap = af.FindAreas();
		this.doors = af.FindAreaDoors(areasMap);
        ///* TMP */
        //string res = "";
        //for (int x=0;x<rsize;x++) {
        //    for (int y=0;y<csize;y++) {
        //        res += areasMap [x * csize + y] + " ";
        //    }
        //    res += "\n";
        //}
        //Debug.Log(res);
        ///* END */
	}

	/**
	 * Builds map from the internal representation.
	 *
	 * LEGEND (provisory)
	 * 	- 'space' : Void
	 *  - @ : Wall
	 *  - . : Floor
	 *  - X : Bot
	 * 
	 * TODO: Add more map items
	 */
	private void BuildMap() {
		LoadMapFromFile();
		for (int i=0;i<rsize;i++) {
			for (int j=0;j<csize;j++) {
				char map_element = staticMap[i*csize+j];
				float[] worldcoords = GetWorldFromIndexes(i,j);
				float x = worldcoords[0];
				float z = worldcoords[1];
				switch (map_element) {
				case '@' :
					Instantiate(wall,new Vector3(x,1,z),Quaternion.identity);
					break;
				case 'D' :
					Instantiate(door,new Vector3(x,1,z),Quaternion.identity);
					break;
				case '.' :
					Instantiate(floor,new Vector3(x,0.05f,z),Quaternion.identity);
					break;
				case 'X' :
					Instantiate(floor,new Vector3(x,0.05f,z),Quaternion.identity);
					Instantiate(bot, new Vector3(x,0.1f,z),Quaternion.Euler(Vector3.up * 90));
                    staticMap[i * csize + j] = '.';
					break;
				default:
					break;
				}
			}
		}	
	}

	/**
	 * Computes matrix indexes from world position.
	 * 
	 * \param x World x coordinate
	 * \param z World z coordinate
	 * \return A size two array with the <i,j> indexes.
	 */
	public int[] GetIndexesFromWorld(float x, float z) {
		int i = (int) (x/gridSize - 0.5);
		int j = (int) (z/gridSize - 0.5);
		int[] res = {i,j};
		return res;
	}

	/**
	 * Returns the world position <x,z> given the index position <i,j>.
	 * 
	 * \param i Matrix i-th row.
	 * \param j Matrix j-th row.
	 * \return The world position <x,z>.
	 */
	public float[] GetWorldFromIndexes(int i, int j) {
		float x = gridSize*(i+0.5f); 
		float z = gridSize*(j+0.5f);
		float[] res = {x, z};
		return res;
    }

	/**
	 * Get the map element in the current world position.
	 * 
	 * \param x World x coordinate.
	 * \param z World z coordinate.
     * \return The element in grid <x,z>.
	 */
	public char GetMapElement(float x, float z) {
		int idx = GetArrayIndex (x, z);
		return staticMap[idx];
	}

    /**
     * Get the map element in the cell <i,j>.
     * 
     * \param i The row.
     * \param j The column.
     * \return The element in <i,j> position.
     */
    public char GetMapElement(int i, int j)
    {
        return staticMap[GetArrayIndex(i, j)];
    }

    /**
     * Get the map element in the current index.
     * 
     * \param idx Linearized array index.
     * \return The element in idx position.
     */
    public char GetMapElement(int idx)
    {
        return staticMap[idx];
    }

    /**
     * Change the map elment in the curent linear array index.
     * 
     * \param idx Linearized array index.
     * \param newElement The new element char.
     */
    public void SetMapElement(int idx, char newElement)
    {
        staticMap[idx] = newElement;
    }

    /**
     * Change the map in the current <i,j> grid position.
     * 
     * \param i The row.
     * \param j The column.
     * \param newElement The new element char.
     */
    public void SetMapElement(int i, int j, char newElement)
    {
        SetMapElement(GetArrayIndex(i, j), newElement);
    }

    /**
     * Change the map in the current world <x,z> position.
     * 
     * \param x World x coordinate.
     * \param z World z coordinate.
     * \param newElement The new element char.
     */
    public void SetMapElement(float x, float z, char newElement)
    {
        SetMapElement(GetArrayIndex(x, z), newElement);
    }

    /**
     * Computes matrix indexes from world position.
     * 
     * \param x World x coordinate
     * \param z World z coordinate
     * \return Index of the linearized map array.
     */
    public int GetArrayIndex(float x, float z)
    {
        int[] idxs = GetIndexesFromWorld(x, z);
        return GetArrayIndex(idxs[0], idxs[1]);
    }

	/**
	 * Converts a pair of matrix indexes <i,j> in the corresponding
	 * index of the linearized array associated to the map matrix.
	 * 
	 * \param i The row index.
	 * \param j The column index.
	 * \return The associated linearized array index.
	 */
	public int GetArrayIndex(int i, int j) {
		return i * csize + j;
	}

	/**
	 * Converts a pair of matrix indexes <i,j> in the corresponding
	 * index of the linearized array associated to the map matrix.
	 * 
	 * \param idxs A pair <i,j> of indexes.
	 * \return The associated linearized array index.
	 */
    public int GetArrayIndex(int[] idxs) {
        return GetArrayIndex(idxs[0], idxs[1]);
	}
    
    /**
     * Convert the linear array index into the original <i,j> pairs.
     * 
     * \param idx The input linearized array index.
     * \return The corresponding <i,j> pair.
     */
    public int[] GetPositionFromArrayIndex(int idx) {
        int[] result = new int[2];
        result[0] = idx/csize;
        result[1] = idx % csize;
        return result;
    }

	/**
	 * Returns the map size.
	 * 
	 * \return A pair <rsize,csize> where rsize is the number of rows in the matrix and
	 * csize is the number of columns.
	 */
	public int[] GetMapSize() {
		int[] res = {this.rsize,this.csize};
		return res;
	}

	/**
	 * Return the area label for the point <i,j>.
	 *
	 * \param i The row index.
	 * \param j The cols index.
	 * \return The label of the <i,j> point.
	 */
	public int GetArea(int i, int j) {
		return areasMap[GetArrayIndex(i,j)];
	}

    /**
     * Return the area label for the world point <x,z>.
     *
     * \param The x coordinate.
     * \param The z coordinate.
     * \return The label of the <x,z> point.
     */
    public int GetArea(float x, float z)
    {
        return areasMap[GetArrayIndex(x, z)];
    }

    /**
     * Return the area label for the index.
     *
     * \param idx The linearized array position.
     * \return The label of the <i,j> point.
     */
    public int GetArea(int idx)
    {
        return areasMap[idx];
    }

    /**
     * Snap the given coordinate to the nearest grid center.
     * 
     * \param x The give coordinate.
     * \return The coordinate of the nearest grid center.
     */
    public float SnapCoord(float x)
    {
        return (float) (Math.Round(x / gridSize - 0.5f) + 0.5f) * gridSize;
    }

	/**
	 * Return the areas connected by the given door.
	 *
	 * \param door The given door.
	 * \return The areas connected by door.
	 */
	public List<int> GetAreasByDoor(int door) {
		return doors[door];
	}

    /**
     * Return the nearest door that connect a1 with a2 (if any).
     * 
     * \param a1 The first area.
     * \param a2 The second area.
     * \param i The bot row.
     * \param j The bot column.
     * \return The nearest door between two areas. Return -1 if no door
     *      can be found.
     */
    public int GetDoorByAreas(int a1, int a2, int i, int j ) {
        if (a1 == a2) return -1;
        List<int> resultDoors = new List<int>();
        foreach (KeyValuePair<int,List<int>> entry in doors) {
            if (doors[entry.Key].Contains(a1) && doors[entry.Key].Contains(a2)) {
                resultDoors.Add(entry.Key);
            }
		}
        if (resultDoors.Count == 0) return -1;
        int result = -1;
        int minDistance = int.MaxValue;
        foreach (int door in resultDoors) {
            int[] doorPos = GetPositionFromArrayIndex(door);
            int distance = Math.Abs(doorPos[0]-i) + Math.Abs(doorPos[1]-j);
            if (distance < minDistance) {
                minDistance = distance;
                result = door;
            }
        }
        return result;
    }

    /**
     * Copy a rectangular region of the static map to the destination array.
     * 
     * The destination array has to be of the same dimention of the "staticMap"
     * array.
     * 
     * \param dest The destination array.
     * \param i The row index of the upper left corner.
     * \param j The column index of the upper left corner.
     * \param width The width of the rectangular region.
     * \param height The height of the rectangular region.
     */
    public void CopyRegion(char[] dest, int i, int j, int width, int height)
    {
        Debug.Log("Copy " + i + " " + j + " - " + width + " " + height);
        // If dest size is not valid do nothing.
        if (dest.Length != staticMap.Length) return;
        // If i or j are out of bound the whole region is out of bound.
        // So do nothing.
        if (i > rsize || j > csize) return;
        // If i or j are negative update width and height to keep
        // them consistent with the original i,j.
        if (i < 0)
        {
            height = height + i;
            i = 0;
        }
        if (j < 0)
        {
            width = width + j;
            j = 0;
        }
        // Do the same if the regio goes out of bounds.
        if (i+height > rsize)
            height = rsize - i;
        if (j + width > csize)
            width = csize - j;
        // Copy the region.
        for (int p = i; p < i + height; p++)
        {
            for (int q = j; q < j + width; q++)
            {
                dest[GetArrayIndex(p, q)] = staticMap[GetArrayIndex(p, q)];
            }
        }
    }
}
