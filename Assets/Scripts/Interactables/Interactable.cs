/* NAME:            Interactable.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     This is the abstract base for all interactable objects by the player which then
 *                  do something when the player interacts with them. Interactables should
 *                  display messages to the player on what is happening unless it is a secret.
 *                  Therefore, it is optimized for dialogues by accessing the UIManager.
 *                  Dialogues can be constructed in a Tree for organization.
 *                  Player.Instance.PlayersTurn should be set to false then true by the Interactable
 *                  if desired.
 * REQUIREMENTS:    None
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    /// <summary>
    /// Nested Dialogue class for Interactables to construct a Dialogue tree.
    /// </summary>
    protected class DialogueNode
    {
        string message;
        List<DialogueNode> children = new List<DialogueNode>();
        
    }

    /// <summary>
    /// This is called by an interactor such as the player
    /// and activates this message, pickup item, etc.
    /// </summary>
    public abstract void Interact();

    /// <summary>
    /// This is the method to be called for the next message,
    /// dialogue node, etc, if necesary. Otherwise, it should
    /// set the Player.Instance.PlayersTurn = true again.
    /// </summary>
    public abstract void Next();
}
