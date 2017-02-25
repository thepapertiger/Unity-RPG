using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TossItemPopUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private bool MouseOver = true;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        MouseOver = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        MouseOver = false;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !MouseOver) {
            UIManager.Instance.TossItemPopUp.SetActive(false);
            UIManager.Instance.SelectedItem = null;
        }
    }
}
