using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentButtons : MonoBehaviour, IPointerDownHandler
{
    public EquipSlots MyEquipSlot;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        //move unequip button near this equipment label and activate it
        UIManager.Instance.UnequipPopUp.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
        UIManager.Instance.UnequipPopUp.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        UIManager.Instance.SelectedEquipSlot = MyEquipSlot;
        UIManager.Instance.UnequipPopUp.SetActive(true);
    }
}