using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillageEnterTrigger : Interactable {

    public Sprite MySprite;
    public Sprite BG;

    public override void Interact()
    {
        if (!GetReceived(this)) { //check if not already received
            GameManager.Instance.Overall.GetComponent<Image>().sprite = BG;
            GameManager.Instance.Overall.SetActive(true);
            CurrentNode = new DialogueNode("Finally my hometown! It's been so long... Now to go find grandma. I hope she didn't wander into the woods like she always does.", null);
            SetReceived(this);
            CurrentNode.Run(this);
        }
    }

}
