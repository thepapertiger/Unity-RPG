using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnequipButton : MonoBehaviour, IPointerDownHandler {

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        UIManager.Instance.SelectedCharacter.GetComponent<Equipment>().Unequip(
            UIManager.Instance.SelectedEquipSlot);
        UIManager.Instance.SelectedBlock.SelectCharacter(); //refresh equipment display
    }

    private void Update()
    {
        if (Input.anyKeyDown)
            gameObject.SetActive(false);
    }

}
