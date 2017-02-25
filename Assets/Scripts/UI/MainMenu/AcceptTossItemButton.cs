using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AcceptTossItemButton : MonoBehaviour, IPointerClickHandler
{
    private int TossQuantity = 1;

    public void UpdateTossQuantity(string quantity)
    {
        TossQuantity = int.Parse(quantity);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (UIManager.Instance.SelectedItem)
            Inventory.Instance.RemoveItem(UIManager.Instance.SelectedItem.Name, TossQuantity);
        else
            Debug.LogError("No item selected!");
        UIManager.Instance.SelectedItem = null;
        UIManager.Instance.TossItemPopUp.SetActive(false);
    }

}