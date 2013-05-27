using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/*!
 * StateBook stores informations about predicative conditions.
 * 
 * For example StateBook can store predicates like `Location(2,3,bot1)` or
 * `Hold(gun)` usefull in deliberative algorithm and tecniques like planning
 * or FSM.
 * 
 * Statebook class is easy to use. To add a conditions you just have to
 * 
 *     statebook["name:arg1 arg2 arg3"] = true;
 *     
 * In the same way we can check the validity of a given conditions with
 * 
 *     statebook["name:arg1 arg2 arg3"];
 *     
 * Statebook use the Closed-World Assumption.
 */
public class StateBook : MonoBehaviour {

    private Dictionary<string, HashSet<ArgsList>> conditionsDB;
    private Dictionary<string, int> predicatesArity;

	// Use this for initialization
	void Start () {
        conditionsDB = new Dictionary<string, HashSet<ArgsList>>();
        predicatesArity = new Dictionary<string, int>();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public bool Query(string name, string[] args) 
    {
        return conditionsDB[name].Contains(new ArgsList(args)); 
    }

    public bool Query(string name, string args)
    {
        return conditionsDB[name].Contains(new ArgsList(args));
    }

    // INDEXERS
    public bool this[string query]
    {
        get
        {
            string[] splitted = query.Split(':');
            return this.Query(splitted[0], splitted[1]);
        }

        set
        {
            string[] splitted = query.Split(':');
            string theName = splitted[0];
            string args = splitted[1];
            int argsNum = args.Split(' ').Length;
            if (value == true)
            {
                if (!conditionsDB.ContainsKey(theName))
                {
                    conditionsDB.Add(theName,new HashSet<ArgsList>());
                    predicatesArity.Add(theName, argsNum);
                }
                if (argsNum != predicatesArity[theName])
                {
                    throw new System.InvalidOperationException("Invalid arguments number!");
                }
                conditionsDB[theName].Add(new ArgsList(args));
            }
            else
            {
                if (!conditionsDB.ContainsKey(theName))
                    return;
                conditionsDB[theName].Remove(new ArgsList(args));
            }
        }
    }
}
