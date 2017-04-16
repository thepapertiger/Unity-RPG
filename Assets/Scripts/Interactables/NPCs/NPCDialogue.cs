using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogue : Interactable
{

    public Sprite MySprite;

    public override void Interact()
    {
        UIManager.Instance.TalkingCharacter.sprite = MySprite;
        UIManager.Instance.DialoguePics.SetActive(true);
        List<DialogueNode> children;
        List<string> qanda;
        if (!GetReceived(this)) {
            DialogueNode ItemNode = new DialogueNode("Bread", 1, null);

            DialogueNode f = new DialogueNode("Yay! Please take my only meal as a gift!", ItemNode);
            DialogueNode g = new DialogueNode("OK, bye bye then, lady!", null);

            children = new List<DialogueNode>();
            children.Add(f);
            children.Add(g);

            qanda = new List<string>();
            qanda.Add("Are you gonna get married?");
            qanda.Add("Why, Yes!");
            qanda.Add("Not really...");

            DialogueNode e = new DialogueNode(qanda, children);

            CurrentNode = new DialogueNode("Wow! Pretty dress!", e);
        }
        else {
            CurrentNode = new DialogueNode("Hello lady! Hopefully mommy finds more food tomorrow! I'm kinda hungry...", null);
        }
        CurrentNode.Run(this);
    }

}