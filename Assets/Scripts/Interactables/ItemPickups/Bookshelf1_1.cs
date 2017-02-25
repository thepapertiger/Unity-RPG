using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bookshelf1_1 : Interactable {

    public override void Interact()
    {
        if (!GetReceived(this)) { //check if not already received
            CurrentNode = new DialogueNode("Mysterious Book", 1, null);
            SetReceived(this);
        }
        else
            CurrentNode = new DialogueNode("The shelf is empty", null);

        CurrentNode.Run(this);
    }

}
