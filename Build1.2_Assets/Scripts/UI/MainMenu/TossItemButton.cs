using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TossItemButton : MonoBehaviour, IPointerClickHandler
{
    public bool IsEnabled = true;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (IsEnabled && UIManager.Instance.SelectedItem) {
            UIManager.Instance.TossItemPopUp.SetActive(true);
        }
    }
}