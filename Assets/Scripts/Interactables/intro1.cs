using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class intro1 : Interactable {

    public override void Interact()
    {
        if (!GetReceived(this)) { //check if not already received
            DialogueNode zzzzz = new DialogueNode("Where am I?... Looks like I'm outside my home village...", null);
            DialogueNode zzzz = new DialogueNode("Awww SHUT UP! This is what the old hag wanted remember? ...", zzzzz);
            DialogueNode zzz = new DialogueNode("You're gonna just leave her here!?", zzzz);
            DialogueNode zz = new DialogueNode("... ... ...", zzz);
            DialogueNode z = new DialogueNode("Ah! Bandits! *fumble* *fumble* ELLA NOOOO!- *thud*", zz);
            DialogueNode y = new DialogueNode("CRASH Ahhhh, what was that? Did we hit a deer?", z);
            DialogueNode x = new DialogueNode("I think it is so brave of you to be doing this so soon after your father--", y);
            DialogueNode w = new DialogueNode("Why hello miss Ella! Hop in the carriage! Hurry hurry! We don't have much time.", x);
            DialogueNode v = new DialogueNode("After today, we will be sisters, you and I.", w);
            DialogueNode u = new DialogueNode("Let's go to the carriage. Gunther is waiting in the feasting hall.", v);
            DialogueNode t = new DialogueNode("You look so beautiful, Ella!", u);
            DialogueNode s = new DialogueNode("... *door opens* Ella! Oh, my!", t);
            CurrentNode = new DialogueNode("It's almost time for the wedding...", s);
            SetReceived(this);
            CurrentNode.Run(this);
        }
    }

}