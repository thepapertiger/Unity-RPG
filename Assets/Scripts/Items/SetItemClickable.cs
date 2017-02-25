/* NAME:            SetItemClickable.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     Item class for every item in game. It is a Scriptable Object.
 * REQUIREMENTS:    Values must be filled out in Unity Editor. Default value for
 *                  integer fields is 0. Currently, only Gold Value
 *                  can be set publicly.
 *                  Please use the case conventions of 'Title Case'. First and Last words
 *                  are always capitalized and every other word except articles (of, the, a).
 *                  Search "Title Case Converter" online, if in doubt.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SetItemClickable : MonoBehaviour, 
    IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemBase MyItem = null;
    private GameObject DragIcon;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        UIManager.Instance.ItemSelectGlow.GetComponent<ItemSelectGlow>().MySelectedItem = this.transform;
        StartCoroutine(WaitFrame());
        //set sprite to show up in details image
        UIManager.Instance.SelectedItem = MyItem;
        UIManager.Instance.MainMenuDetailsImage.sprite =
            UIManager.Instance.SelectedItem.Sprite;
        UIManager.Instance.MainMenuDetailsText.text =
            UIManager.Instance.SelectedItem.Name + "\n"
            + UIManager.Instance.SelectedItem.Description;
    }

    /// <summary>
    /// "Moving glow" bug fix. Ensures no display until the glow is in position.
    /// </summary>
    IEnumerator WaitFrame()
    {
        yield return new WaitForEndOfFrame();
        UIManager.Instance.ItemSelectGlow.SetActive(true);
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        DragIcon = Inventory.Instance.DragImage;
        DragIcon.GetComponent<Image>().sprite = MyItem.Sprite;
        DragIcon.SetActive(true);
        UIManager.Instance.IsDraggingItem = true;
        UIManager.Instance.PartyArea.SetActive(true);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        DragIcon.transform.position = eventData.position;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        DragIcon.SetActive(false);
        UIManager.Instance.PartyArea.SetActive(false);
        StartCoroutine(Wait());
    }

    /// <summary>
    /// This waits for any party member to finish equipping item, then sets flag to false after.
    /// </summary>
    private IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();
        UIManager.Instance.IsDraggingItem = false;
    }
}
