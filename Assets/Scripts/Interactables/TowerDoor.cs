using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerDoor : Interactable
{

    public Sprite MySprite;
    public Sprite BG;

    public override void Interact()
    {
        if (!GetReceived(this)) { //check if not already received
            GameManager.Instance.Overall.GetComponent<Image>().sprite = BG;
            GameManager.Instance.Overall.SetActive(true);
            DialogueNode n = new DialogueNode("Credits: Catherine, Sierra Johnson, Angela Lerias, Marjie, Shinlynn Kuo, Jeffrey Cheng, Kevin Huang, and Emmilio Segovia. -Gamespawn", null);
            CurrentNode = new DialogueNode("I finally found the Warden's office! The communication device must be inside... Hopefully you play the full game to find out what happens! ...TO BE CONTINUED.", n);
            SetReceived(this);
            CurrentNode.Run(this);
        }
    }

}
