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
 * REQUIREMENTS:    If RunDialogue() is called, the dialogue tree must
 *                  call QuitDialogue() when done.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    /*
    /// <summary>
    /// Nested Dialogue class for Interactables to construct a Dialogue tree.
    /// </summary>
    protected class DialogueNode
    {
        private string message;
        private DialogueNode NextNode; //if next message is not optional
        private List<DialogueNode> responses = new List<DialogueNode>();//if next depends on input
        private string ItemName;
        private int ItemQuantity = 0; //default: flag that there is no item

        /// <summary>
        /// Indicate "null" as next if no more Nodes.
        /// </summary>
        public DialogueNode(string new_message, DialogueNode next_node)
        {
            message = new_message;
            NextNode = next_node;
        }
        /// <summary>
        /// The item will be added to the Inventory when this dialogue's Next() is called.
        /// item_quantity is optinoal. If you do not indicate quantity, 1 will be given.
        /// </summary>
        public DialogueNode(string item_name, int item_quantity = 1, DialogueNode next_node)
        {
            ItemName = item_name;
            ItemQuantity = item_quantity;
            NextNode = next_node;
        }
        /// <summary>
        /// Dialogue coroutine calls this when done to get the next Node to activate.
        /// </summary>
        public DialogueNode Next()
        {
            if (ItemQuantity > 0)
                Inventory.Instance.AddItem()
            return NextNode;
        }
    }*/

    /// <summary>
    /// This is called by an interactor such as the player
    /// and activates this message, pickup item, etc.
    /// </summary>
    public abstract void Interact();

    /// <summary>
    /// This is the method to be called for the next message,
    /// dialogue node, etc, if necesary. Otherwise, it should
    /// call QuitDialogue().
    /// </summary>
    public abstract void Next();

    /// <summary>
    /// Call this to Display a Dialogue. Shortens code for all the Interactables.
    /// </summary>
    protected void RunDialogue(Interactable caller, string message)
    {
        UIManager.Instance.RunDialogue(this, message);
    }

    /// <summary>
    /// This method sets the game state back to idle from DialogueState
    /// </summary>
    protected void QuitDialogue()
    {
        UIManager.Instance.DialogueCanvas.SetActive(false);
        GameManager.Instance.SetState(GameStates.IdleState);
    }

    /// <summary>
    /// Call this if you are adding an item to inventory.
    /// quantity parameter is optional, the default = 1 if not indicated.
    /// </summary>
    protected void AddItem(Interactable caller, string item_name,int quantity = 1) 
    {
        int result = Inventory.Instance.AddItem(item_name, quantity);
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
        return GameManager.Instance.GetReceived(caller.ToString());
    }

    /// <summary>
    /// Records that items were received from caller so that they cannot be received again.
    /// </summary>
    protected void SetReceived(Interactable caller)
    {
        GameManager.Instance.SetReceived(caller.ToString());
    }
}
