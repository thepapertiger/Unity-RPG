using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrisonEnter : Interactable
{

    public Sprite MySprite;
    public Sprite BG;

    public override void Interact()
    {
        if (!GetReceived(this)) { //check if not already received
            GameManager.Instance.Overall.GetComponent<Image>().sprite = BG;
            GameManager.Instance.Overall.SetActive(true);
            CurrentNode = new DialogueNode("The Court of Aesir's prison. I must find the communication device to reach Gunther!", null);
            SetReceived(this);
            CurrentNode.Run(this);
        }
    }

}
