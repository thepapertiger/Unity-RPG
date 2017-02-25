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

    public string message1 = "The door is locked. Why are you still standing there like " +
        "an idiot? Can't you read the door is locked. Oh and by the way, notice how this " +
        "long text wraps?";

    public override void Interact()
    {
        List<DialogueNode> children;
        List<string> qanda;
        if (!GetReceived(this)) {
            DialogueNode ItemNode = new DialogueNode("Bread", 20, null);

            DialogueNode f = new DialogueNode("Hai dozo! 20 BREADDD!", ItemNode);
            DialogueNode g = new DialogueNode("ok bruh", null);
            DialogueNode h = new DialogueNode("Why you greedy little--! You get nothing! BEGONE!", null);

            children = new List<DialogueNode>();
            children.Add(f);
            children.Add(g);
            children.Add(h);
            qanda = new List<string>();
            qanda.Add("Would you like an item?");
            qanda.Add("Mmmkayyy!");
            qanda.Add("nah man");
            qanda.Add("What kind of item?");
            DialogueNode e = new DialogueNode(qanda, children);

            DialogueNode d = new DialogueNode("Goodbye Then, Mortal...", null);
            DialogueNode c = new DialogueNode("You are kind...", e);

            children = new List<DialogueNode>();
            children.Add(d);
            children.Add(c);
            qanda = new List<string>();
            qanda.Add("Will you walk away?");
            qanda.Add("yes");
            qanda.Add("no");

            DialogueNode b = new DialogueNode(qanda, children);
            CurrentNode = new DialogueNode("The door is locked...", b);
        }
        else 
        {
            DialogueNode c = new DialogueNode("You are a greedy little bastard. BEGONE!", null);
            DialogueNode d = new DialogueNode("I see, so you came just to vist. How nice!", null);

            children = new List<DialogueNode>();
            children.Add(c);
            children.Add(d);
            qanda = new List<string>();
            qanda.Add("Do you want another item?");
            qanda.Add("yes'um");
            qanda.Add("Of course not!");

            DialogueNode b = new DialogueNode(qanda, children);
            CurrentNode = new DialogueNode("You're back so soon!", b);
        }
        CurrentNode.Run(this);
    }

}