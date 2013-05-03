using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**
 * This class is a collection of low-level actions that a bot can do.
 * 
 * These actions can be invoked by BotControl to perform high-level actions.
 * 
 * The avaiable actions are:
 * 	- `move x z` : Move the bot to the <x,0,z> world position.
 *  - `lookat x z` : Look at the <x,0,z> point.
 *  - `grab` : Grab an item in the current position.
 * 
 * TODO: Complete Action Specification
 * 
 * \author Davide Aversa
 * \version 1.0
 * \date 2013
 * \pre This class needs an instance of GridWorldMap and BotControl.
 */
[RequireComponent(typeof(BotControl))]
public class BotActions : GridWorldBehaviour {

    private Dictionary<string,Action<string[]>> actions;
    private Action abortAction;

	private bool actionComplete = true;		/**< True if the last action is completed. */
	private bool actionSuccess = true;		/**< True if the last action is completed successfully. */ 

	private BotControl parentControl;		/**< A reference to a BotControl instance. */


	// Use this for initialization
	protected override void Awake () {
        base.Awake();
		parentControl = gameObject.GetComponent<BotControl>();
        actions = new Dictionary<string, Action<string[]>>();
	}

    public void RegisterNewAction(string command, Action<string[]> action)
    {
        Debug.Log("BotACTIONS: Registering " + command + " command");
        actions[command] = action;
    }

    public void RegisterAbortAction(Action abortAction)
    {
        this.abortAction = abortAction;
    }

	/**
	 * Perform the given action (if exists).
	 * 
	 * \param action The action that must be executed.
	 * \retval true If the action can be executed.
	 * \retval false If the action can not be executed.
	 */
    public bool DoAction(string fullCommand)
    {
        Debug.Log("Action Received: " + fullCommand);
        string[] commands = fullCommand.Split(' ');
        if (fullCommand == "stop")
        {
            abortAction.Invoke();
            return true;
        }
        if (actionComplete && actions.ContainsKey(commands[0]))
        {
            Debug.Log("FUCK!");
            actionComplete = false;
            actionSuccess = false;
            actions[commands[0]].Invoke(commands);
        }
        return false;
    }

	/*!
	 * Check if the last action is completed.
	 * 
	 * \retval true If the last action is completed.
	 * \retval false If the last action is still running.
	 */
	public bool LastActionComplete() {
		return actionComplete;
	}

	/**
	 * Check if the last action is completed succesfully.
	 * 
	 * \retval true If the last action is completed succesfully.
	 * \retval false If the last action is not completed or completed with faliure.
	 */
	public bool LastActionCompletedSuccessfully() {
		return actionComplete && actionSuccess;
	}

    public void NotifyActionComplete()
    {
        actionComplete = true;
    }

    public void NotifyActionSuccess()
    {
        actionSuccess = true;
    }

    public void NotifyAction(string action)
    {
        parentControl.NotifyAction(action);
    }

}
