/* NAME:            Interactable.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     This is the abstract base for all interactable objects by the player which then
 *                  do something when the player interacts with them. Interactables should
 *                  display messages to the player on what is happening unless it is a hidden.
 *                  Therefore, it is optimized for dialogues by accessing the UIManager.
 *                  Dialogues can be constructed in a Tree for organization.
 *                  
 *                  To add Items to Inventory, in Interact(), call:
 *                          AddItem(item_name, this);
 *                      
 * REQUIREMENTS:    If RunDialogue() is called explicitly since a tree is not needed, 
 *                  QuitDialogue() must be called when done. otherwise, the next function
 *                  defined here will take care of it if a tree is used.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour
{
    /// <summary>
    /// This is to be set as root of the dialogue tree. 
    /// It is meant to minize code in each Interactable.
    /// </summary>
    protected DialogueNode CurrentNode = null;

    /// <summary>
    /// This is called by an interactor such as the player
    /// and activates this message, pickup item, etc.
    /// </summary>
    public abstract void Interact();

    /// <summary>
    /// Returns the default next child of the current node.
    /// </summary>
    public void Next(Interactable caller)
    {
        if (CurrentNode.HasNext()) {
            CurrentNode = CurrentNode.GetNext();
            CurrentNode.Run(caller);
        }
        else
            QuitDialogue();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Next(Interactable caller, int answer)
    {
        UIManager.Instance.SelectAnswer(-1); //ensure answer selection is reset in UIManager
        if (CurrentNode.HasNext()) {
            CurrentNode = CurrentNode.GetNext(answer);
            CurrentNode.Run(caller);
        }
        else
            QuitDialogue();
    }

    /// <summary>
    /// Call this to Display a Dialogue. Shortens code for all the Interactables.
    /// </summary>
    protected void RunDialogue(Interactable caller, string message)
    {
        UIManager.Instance.RunDialogue(this, message);
    }

    /// <summary>
    /// This must be called to return tell the gamme state machine that dialogue is over
    /// </summary>
    protected void QuitDialogue()
    {
        UIManager.Instance.DialogueCanvas.SetActive(false);
        if (GameManager.Instance.IsState(GameStates.DialogueState))
            GameManager.Instance.SetState(GameStates.IdleState);
    }

    /// <summary>
    /// Call this if you are adding an item to inventory.
    /// quantity parameter is optional, the default = 1 if not indicated.
    /// </summary>
    public static void AddItem(Interactable caller, string item_name,int quantity = 1) 
    {
        int result = Inventory.Instance.AddItem(item_name, quantity);
        UIManager.Instance.AddedItemFrame.gameObject.SetActive(true);
        UIManager.Instance.AddedItemFrame.transform.GetChild(0).GetComponent<Image>().sprite =
            ResourceManager.Instance.GetItem(item_name).Sprite;
        string message = "You got " + item_name;

        if (quantity > 1) //only display quantity if > 1
            message += " x" + quantity;
        message += " ("+ ResourceManager.Instance.GetItem(item_name).Type.ToString() + ")";

        if (result == 0)
            message += "!";
        else if (result == 1) {
            message += ", but "
                + ResourceManager.Instance.GetItem(item_name).Type.ToString()
                + " pocket is full.";
        }
        else if (result == 2)
            message += ", but you cannot carry anymore!";

        UIManager.Instance.RunDialogue(caller, message);
    }

    /// <summary>
    /// Returns whether or not items were already received from the caller.
    /// </summary>
    protected bool GetReceived(Interactable caller)
    {
        return GameManager.Instance.GetReceived(caller.GetType().ToString());
    }

    /// <summary>
    /// Records that items were received from caller so that they cannot be received again.
    /// </summary>
    protected void SetReceived(Interactable caller)
    {
        GameManager.Instance.SetReceived(caller.GetType().ToString());
    }
}
