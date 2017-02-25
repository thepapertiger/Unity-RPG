/* NAME:            Doormessage.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     This is a test script for an interactable door.
 * REQUIREMENTS:    None
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMessage : Interactable {

    private string message = "The door is locked. Why are you still standing there like " +
        "an idiot? Can't you read the door is locked. Oh and by the way, notice how this " +
        "long text wraps?";

	public override void Interact () {
        Player.Instance.PlayersTurn = false; //temporarily disable Players
        UIManager.Instance.RunDialogue(message, this);
	}

    public override void Next()
    {
        Player.Instance.PlayersTurn = true; //No more dialogue Nodes
    }
}
