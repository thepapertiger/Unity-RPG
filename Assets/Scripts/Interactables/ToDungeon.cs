using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToDungeon : Interactable
{

    public Sprite MySprite;
    public Sprite BG;

    public override void Interact()
    {
            GameManager.Instance.DialogueFrame.sizeDelta = new Vector2(700, 70);
            SoundManager.Instance.SetMusic(ResourceManager.Instance.GetSound("PrisonMusic"));
            //GameManager.Instance.Overall.GetComponent<Image>().sprite = BG;
            //GameManager.Instance.Overall.SetActive(true);
            CurrentNode = new DialogueNode("~~~The Court of Aesir Prison~~~", null);
            //SetReceived(this);
            CurrentNode.Run(this);
        
    }

}
