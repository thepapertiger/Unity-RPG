using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CancelTossItemButton : MonoBehaviour, IPointerClickHandler
{
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        UIManager.Instance.TossItemPopUp.SetActive(false);
        UIManager.Instance.SelectedItem = null;
    }
}