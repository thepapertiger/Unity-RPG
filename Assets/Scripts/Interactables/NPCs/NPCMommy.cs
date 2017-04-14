using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCMommy : Interactable {

    public Sprite MySprite;

    public override void Interact()
    {
        UIManager.Instance.TalkingCharacter.sprite = MySprite;
        UIManager.Instance.DialoguePics.SetActive(true);
        List<DialogueNode> children;
        List<string> qanda;
        if (!GetReceived(this)) {
            DialogueNode ItemNode = new DialogueNode("Bread", 1, null);

            DialogueNode f = new DialogueNode("Oh! Please give this to my daughter! I beg of you...", ItemNode);
            DialogueNode g = new DialogueNode("...I see...", null);

            children = new List<DialogueNode>();
            children.Add(f);
            children.Add(g);

            qanda = new List<string>();
            qanda.Add("Will you please look for her and give her this food?");
            qanda.Add("Of course.");
            qanda.Add("I'm kinda busy...");

            DialogueNode e = new DialogueNode(qanda, children);

            CurrentNode = new DialogueNode("Miss, please... I can't find my daughter. She hasn't eaten in days...", e);
        }
        else {
            CurrentNode = new DialogueNode("Please find my daughter...", null);
        }
        CurrentNode.Run(this);
    }

}