using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox1_1 : Interactable {

    public override void Interact()
    {
        if (!GetReceived(this)) { //check if not already received
            DialogueNode c = new DialogueNode("Chubby Sword", 2, null);
            DialogueNode b = new DialogueNode("Dauthodagr", 1, c);
            CurrentNode = new DialogueNode("Mysterious Book", 1, b);
            SetReceived(this);
        }
        else
            CurrentNode = new DialogueNode("The box is empty", null);

        CurrentNode.Run(this);
    }

}
