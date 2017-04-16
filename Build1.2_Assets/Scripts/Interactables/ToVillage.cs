using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToVillage : Interactable
{

    public Sprite MySprite;
    public Sprite BG;

    public override void Interact()
    {
            GameManager.Instance.DialogueFrame.sizeDelta = new Vector2(700, 70);

        SoundManager.Instance.SetMusic(ResourceManager.Instance.GetSound("CastleMusic"));
            //GameManager.Instance.Overall.GetComponent<Image>().sprite = BG;
            //GameManager.Instance.Overall.SetActive(true);
            CurrentNode = new DialogueNode("~~~The Rebel Village~~~", null);
            //SetReceived(this);
            CurrentNode.Run(this);
        
    }

}
