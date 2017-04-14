using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrisonCell : Interactable
{

    public Sprite MySprite;
    public Sprite BG;

    public override void Interact()
    {
        if (!GetReceived(this)) { //check if not already received
            //GameManager.Instance.DialogueFrame.sizeDelta = new Vector2(700, 70);
            GameManager.Instance.Overall.GetComponent<Image>().sprite = BG;
            GameManager.Instance.Overall.SetActive(true);
            CurrentNode = new DialogueNode("How terrible! You can hear the groans and cries for help from all the torture.", null);
            SetReceived(this);
            CurrentNode.Run(this);
        }
    }

}
