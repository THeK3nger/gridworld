using UnityEngine;
using System.Collections;
using Pathfinding;

/**
 * This class is a collection of low-level actions that a bot can do.
 * 
 *  * The avaiable actions are:
 * 	- `move x z` : Move the bot to the <x,0,z> world position.
 *  - `lookat x z` : Look at the <x,0,z> point.
 *  - `grab` : Grab an item in the current position.
 *  - `drop` : Unload the gold in the area (if the area is valid).
 */
[RequireComponent(typeof(BotActions))]
public class DiagAction : GridWorldBehaviour
{

    private BotAttributes attributes;
    private BotActions parentAction;

    public float moveBaseSpeed = 2; 		/**< Walk speed in m/s. */
    public float speedDecreaseRate;         /**< Speed decrease rate by gold. */

    public AudioClip goldGrab;

	// Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        attributes = gameObject.GetComponent<BotAttributes>();
        parentAction = gameObject.GetComponent<BotActions>();
	}

    void Start()
    {
        parentAction.RegisterAbortAction(AbortCurrentAction);
        parentAction.RegisterNewAction("move", MoveTo);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AbortCurrentAction()
    {
        if (!parentAction.LastActionComplete())
        {
            // If there are any animation running, stop it.
            iTween.Stop(gameObject);
            // Snap to the nearest grid point.
            Vector3 current = gameObject.transform.position;
            MoveTo(mapWorld.SnapCoord(current.x), mapWorld.SnapCoord(current.z));
        }
    }

    /**
     * Move the bot to the world plane <x,z> position.
     * 
     * \param x Desired x world location.
     * \param z Desired z world location.
     */
    void MoveTo(float x, float z)
    {
        Vector3 target = new Vector3(x, 0, z);
        (gameObject.GetComponent("Seeker") as Seeker).StartPath(gameObject.transform.position, target, this.PathFoundCallback);
    }

    /**
     * Move the bot to the world plane <x,z> position.
     * 
     * \param moveCommand the parsed move command.
     */
    void MoveTo(string[] moveCommand)
    {
        float x = float.Parse(moveCommand[1]);
        float z = float.Parse(moveCommand[2]);
        MoveTo(x, z);
        //MoveToWithoutPathfinding(x, z);
    }

    void MoveToWithoutPathfinding(float x, float z)
    {
        Vector3 target = new Vector3(x, 0, z);
        int[] sizes = mapWorld.GetMapSize();
        if (x < 0 || x > sizes[0]) return;
        if (z < 0 || z > sizes[1]) return;
        float moveSpeed = moveBaseSpeed * (float)System.Math.Exp(-speedDecreaseRate * attributes.goldCarrying);
        animation.CrossFade("walk");
        iTween.MoveTo(gameObject, iTween.Hash
              (
            "position", target,
            "orienttopath", true,
            "looktime", 1.0,
            "lookahead", 0.05,
            "axis", "y",
            "y", 1,
            "easetype", iTween.EaseType.linear,
            "time", 1 / moveSpeed,
            "oncomplete", "onMoveToPathComplete"
            ));
    }

    /**
     * Calback called by Aron Pathfinding Algorithm when a path is available.
     * 
     * \param path The desired path.
     */
    void PathFoundCallback(Path path)
    {
        float moveSpeed = moveBaseSpeed * (float)System.Math.Exp(-speedDecreaseRate * attributes.goldCarrying);
        moveSpeed = System.Math.Max(0.5f, moveSpeed);
        animation.CrossFade("walk");
        Vector3[] array_path = path.vectorPath.ToArray();
        iTween.MoveTo(gameObject, iTween.Hash
                      (
            "path", array_path,
            "orienttopath", true,
            "looktime", 1.0,
            "lookahead", 0.05,
            "axis", "y",
            "y", 1,
            "easetype", iTween.EaseType.linear,
            "time", iTween.PathLength(array_path) / moveSpeed,
            "oncomplete", "onMoveToPathComplete"
            ));
    }

    /**
     * Calback called by iTween when the path execution is completed.
     */
    void onMoveToPathComplete()
    {
        animation.CrossFade("idle1");
        Grab(null);
        parentAction.NotifyActionComplete();
        parentAction.NotifyActionSuccess();
        parentAction.NotifyAction("move");
    }

    /**
     * Look at the `dir` world point.
     * 
     * \param dir The point that we want to look at.
     */
    void LookAt(Vector3 dir)
    {
        iTween.LookTo(gameObject, dir, 1.0f);
    }

    /**
     * Attack
     */
    void Attack()
    {

    }

    /**
     * Grab the object in current location.
     */
    void Grab(string[] command)
    {
        Vector3 current = gameObject.transform.position;
        char currentItem = mapWorld.GetMapElement(current.x, current.z);
        if (mapWorld.ElementIs("collectable", currentItem))
        {
            DestroyGameObjectByPosition(current, 'G');
            attributes.goldCarrying += 1;
            mapWorld.SetMapElement(current.x, current.z, '.');
            AudioSource.PlayClipAtPoint(goldGrab, transform.position);
            parentAction.NotifyAction("grab");
            parentAction.NotifyActionSuccess();
        }
        else
        {
            Debug.Log("Nothing to Grab!!!");
        }
        // TODO: How to invoke a return value?
    }

    /**
     * Drop all the gold in the deposit.
     */
    void Drop(string[] command)
    {
        Vector3 current = gameObject.transform.position;
        if (mapWorld.GetArea(current.x, current.z) == attributes.spawnArea)
        {
            attributes.goldStored += attributes.goldCarrying;
            attributes.goldCarrying = 0;
            Debug.Log("SCORE: " + attributes.goldStored);
            parentAction.NotifyAction("drop");
            parentAction.NotifyActionSuccess();
        }
        parentAction.NotifyActionComplete();
    }

    /**
     * Destroy an object of a given type in the given position.
     * 
     * \param position The desired object position.
     * \param type The desired object type.
     */
    private void DestroyGameObjectByPosition(Vector3 position, char type)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, 1.5f);
        foreach (Collider c in hitColliders)
        {
            SmartObjects so = c.gameObject.GetComponent<SmartObjects>();
            if (so != null)
            {
                if (so.type[0] == type &&
                    System.Math.Abs(c.gameObject.transform.position.x - position.x) < 0.1 &&
                    System.Math.Abs(c.gameObject.transform.position.z - position.z) < 0.1)
                {
                    Destroy(c.gameObject);
                    return;
                }
            }
        }
    }
}
