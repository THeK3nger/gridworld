using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * An utility class for automatic areas extraction from a bidimentional map.
 * 
 * \author Davide Aversa
 * \version 1.0
 * \date 2013
 * 
 * TODO: The argument should be a GridWorldMap instance.
 */
public class AreaFinder {

    private char[] input;               /**< The input map. */
	private int rsize;                  /**< The number of rows. */
	private int csize;                  /**< The number of columns. */
    private GridWorldMap mapWorld;       /**< A link to the original GridWorldMap. */

	private Dictionary<int,int> equivalence; /**< A dictionary that stores the equivalence class between labels. */

    /**
     * Constructor.
     * 
     * /param input The input chars map.
     * /param rsize The number of rows in the map.
     * /param csize The number of columns in the map.
     * /param mapWorld A link to the original GridWorldMap class.
     */
	public AreaFinder(char[] input, int rsize, int csize, GridWorldMap mapWorld) {
		this.input = input;
		this.rsize = rsize;
		this.csize = csize;
		equivalence = new Dictionary<int,int>();
        this.mapWorld = mapWorld;
	}

	/**
	 * Return true if and only if the char `c` is a door.
	 */
	private bool isDoor(char c) {
		return mapWorld.ElementIs("doors",c);
	}

	/**
	 * Return true if and only if the char `c` is a walkable.
	 */
	private bool isWalkable(char c) {
		return mapWorld.ElementIs("walkable",c);
	}

	/**
	 * Converts a pair of matrix indexes <i,j> in the corresponding
	 * index of the linearized array associated to the map matrix.
	 * 
	 * \param i The row index.
	 * \param j The column index.
	 * \return The associated linearized array index.
	 *
	 * TODO: Duplication from GridWorldMap! Fix this!
	 */
	public int GetArrayIndex(int i, int j) {
		return i * csize + j;
	}

	/**
	 * Return a list of labelled neightbour of a give cell.
	 * 
	 * \param i The row index.
	 * \param j The column index.
	 * \param labelled The partially labelled map.
	 * \return The list of labelled neighbours.
	 */
	private List<int> GetNeighbours(int i, int j, int[] labelled) {
		List<int> res = new List<int>();
		int neighbourvalue = 0;
		if (i-1 >= 0) {
			neighbourvalue = labelled[GetArrayIndex(i-1,j)];
			if (neighbourvalue != 0)
				res.Add(neighbourvalue);
		}
		if (j-1 >= 0) {
			neighbourvalue = labelled[GetArrayIndex(i,j-1)];
			if (neighbourvalue != 0)
				res.Add(neighbourvalue);
		}
		if (i+1 < rsize) {
			neighbourvalue = labelled[GetArrayIndex(i+1,j)];
			if (neighbourvalue != 0)
				res.Add(neighbourvalue);
		}
		if (j+1 < csize) {
			neighbourvalue = labelled[GetArrayIndex(i,j+1)];
			if (neighbourvalue != 0)
				res.Add(neighbourvalue);
		}
		return res;
	}

	/**
	 * Join two label in the same equivalence class.
	 * 
	 * Map label1 into label2.
	 *
	 * \param label1 The first label.
	 * \param label2 The target label.
	 */
	private void JoinLabel(int label1, int label2) {
		if (label1 <= label2) return;
		if (!equivalence.ContainsKey(label1)) {
			equivalence.Add(label1,label2);
		} else {
			JoinLabel(label2,equivalence[label1]);
		}
	}

	/**
	 * Navigate the label equivalences to the root label.
	 *
	 * \param label.
	 * \return The root label equivalent to `label`.
	 */
	private int GetParentLabel(int label) {
		int current_label = label;
		int block = 0;
		while (equivalence.ContainsKey(current_label) && block < 10) {
			current_label = equivalence[current_label];
			block++;
		}
		return current_label;
	}

	/**
	 * find the list minimum value.
	 *
	 * \param list Input list.
	 * \return The list minimum
	 */
	private int ListMin(List<int> list) {
		int min = 10000;
		foreach (int i in list) {
			if (i<min)
				min = i;
		}
		return min;
	}

	/**
	 * Find all the connencted areas in the map.
     * 
     * Based on the Connected-Component Labelling Algorithm.
	 *
	 * \return A labelled version of the input map.
	 */
	public int[] FindAreas() {
		int[] result = new int[csize*rsize];
		int next_label = 1;
		Debug.Log("FINDAREAS: First Pass");
		// First Pass
		for (int i=0;i<rsize;i++) {
			for (int j=0;j<csize;j++) {
				char current = input[GetArrayIndex(i,j)];
				if (isWalkable(current)) {
					List<int> neightbours = GetNeighbours(i,j,result);
					if (neightbours.Count == 0) { // No labelled neightbours
						result[GetArrayIndex(i,j)] = next_label;
						next_label++; //Mark with new label.
					} else {
						int min = ListMin(neightbours);   // Mark with the minimum neighbour value.
						result[GetArrayIndex(i,j)] = min; // and join the two label in the same equivalence class.
						foreach (int l in neightbours) {
							if (l != min) JoinLabel(l,min);
						}
					}
				}
			}
		}
		// Second Pass
		Debug.Log("FINDAREAS: Second Pass");
		for (int i=0;i<rsize;i++) {
			for (int j=0;j<csize;j++) {
				int current_label = result[GetArrayIndex(i,j)];
				if (current_label != 0) {
					result[GetArrayIndex(i,j)] = GetParentLabel(result[GetArrayIndex(i,j)]);
				}
			}
		}

		return result;
	}

	/**
	 * Extract from the labelled map a list of doors.
	 *
	 * A door is a walkable point where two or more areas are connected.
	 *
	 * \param labelled The labelled map version.
	 * \return A dictionary that map a door with the list of connected areas.
	 */
	public Dictionary<int,List<int>> FindAreaDoors(int[] labelled) {
		Dictionary<int,List<int>> result = new Dictionary<int,List<int>>();
		for (int i=0;i<rsize;i++) {
			for (int j=0;j<csize;j++) {
				char current = input[GetArrayIndex(i,j)];
				if (isDoor(current)) {
					List<int> neightbours = GetNeighbours(i,j,labelled);
					result.Add(GetArrayIndex(i,j),neightbours);
				} 
			}
		}
		return result;
	}

}
