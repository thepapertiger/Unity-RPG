using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuPartyBlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler, IPointerDownHandler
{
    //update MyStats when Character is changed.
    public GameObject Character;
    public Stats MyStats;
    private Equipment MyEquip;
    private List<Text> TempText = new List<Text>(); //references to all Text changed due to item hover
    private List<string> SavedText = new List<string>(); //saved text of TempText list; revert on end drag
    private int index = 0;
    private ItemBase SelectedItem;
    private int StatChange;  //avoid creating this variable too often
    private int NewStat; //avoid creating this variable too often

    /// <summary>
    /// Returns whether this item is equippable or not by the character represented on this block
    /// </summary>
    public bool Equippable(ItemBase equipment)
    {
        SelectedItem = UIManager.Instance.SelectedItem;
        return (MyStats.Level >= SelectedItem.MinimumLevel && Character.GetComponent<Equipment>());
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        SelectedItem = UIManager.Instance.SelectedItem; //shorten code
        if (!MyEquip) //hold reference for fast access
            MyEquip = Character.GetComponent<Equipment>();

        if (UIManager.Instance.IsDraggingItem && 
                    SelectedItem.Type != ItemTypes.Material && SelectedItem.Type != ItemTypes.Key &&
                    Equippable(SelectedItem)) {
            UIManager.Instance.MainMenuDetailsText.text = "";
            SelectCharacter();
            switch (SelectedItem.Type) {
                case (ItemTypes.Consumable):
                    if (MyStats.HP < MyStats.MaxHP && SelectedItem.HPRecovery > 0) { //check if theres a change
                        UpdateStatDisplay("HP", MyStats.HP, SelectedItem.HPRecovery, MyStats.MaxHP);
                    }
                    if (MyStats.MP < MyStats.MaxMP && SelectedItem.MPRecovery > 0) {
                        UpdateStatDisplay("MP", MyStats.MP, SelectedItem.MPRecovery, MyStats.MaxMP);
                        }
                        break;
                case (ItemTypes.Weapon):
                    if (SelectedItem.AttackValue != 0 || (MyEquip.GetEquipSlot(SelectedItem.EquipSlot) &&
                            MyEquip.GetEquipSlot(SelectedItem.EquipSlot).AttackValue != 0)) {
                        NewStat = SelectedItem.AttackValue;
                        if (MyEquip.GetEquipSlot(SelectedItem.EquipSlot))
                            NewStat -= MyEquip.GetEquipSlot(SelectedItem.EquipSlot).AttackValue;
                        UpdateStatDisplay("STR", MyStats.Strength, NewStat);
                    }
                    if (SelectedItem.AgilityValue != 0 || (MyEquip.GetEquipSlot(SelectedItem.EquipSlot) &&
                            MyEquip.GetEquipSlot(SelectedItem.EquipSlot).AgilityValue != 0)) {
                        NewStat = SelectedItem.AgilityValue;
                        if (MyEquip.GetEquipSlot(SelectedItem.EquipSlot))
                            NewStat -= MyEquip.GetEquipSlot(SelectedItem.EquipSlot).AgilityValue;
                        UpdateStatDisplay("AGL", MyStats.Agility, NewStat);
                    }
                    break;
                case (ItemTypes.Gear):
                    if (SelectedItem.DefenseValue != 0 || (MyEquip.GetEquipSlot(SelectedItem.EquipSlot) &&
                            MyEquip.GetEquipSlot(SelectedItem.EquipSlot).DefenseValue != 0)) {
                        NewStat = SelectedItem.DefenseValue;
                        if (MyEquip.GetEquipSlot(SelectedItem.EquipSlot))
                            NewStat -= MyEquip.GetEquipSlot(SelectedItem.EquipSlot).DefenseValue;
                        UpdateStatDisplay("DEF", MyStats.Defense, NewStat);
                    }
                    if (SelectedItem.AgilityValue != 0 || (MyEquip.GetEquipSlot(SelectedItem.EquipSlot) &&
                            MyEquip.GetEquipSlot(SelectedItem.EquipSlot).AgilityValue != 0)) {
                        NewStat = SelectedItem.AgilityValue;
                        if (MyEquip.GetEquipSlot(SelectedItem.EquipSlot))
                            NewStat -= MyEquip.GetEquipSlot(SelectedItem.EquipSlot).AgilityValue;
                        UpdateStatDisplay("AGL", MyStats.Agility, NewStat);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Temporarily updates the display of stats in menu to show potential changes.
    /// max_stat is an optional parameter for HP, MP, EXP, otherwise it will display using '->'.
    /// </summary>
    private void UpdateStatDisplay(string stat, int current_stat, int new_stat, int max_stat = -1)
    {
        TempText.Add(transform.FindChild(stat).GetComponent<Text>()); //get text displayed
        SavedText.Add(TempText[index].text); //save for quick undo
            StatChange = current_stat + new_stat; //get increase/decrease
        if (max_stat >= 0 && StatChange > max_stat)
            StatChange = max_stat;
        //display increase/decrease
        if (max_stat < 0)
            TempText[index].text += "->" + StatChange;
        else
            TempText[index].text = StatChange + "/" + max_stat;
        //set text color based on positive/negative change (green/red)
        StatChange -= current_stat; //final - inital = positive or negative change
        if (StatChange > 0)
            TempText[index].color = new Color(0.1f, 0.5f, 0.1f); //dark green
        else if (StatChange < 0)
            TempText[index].color = new Color(0.5f, 0.1f, 0.1f); //dark red
        index++;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        for (int i = 0; i < TempText.Count; i++) {
            TempText[i].text = SavedText[i];
            TempText[i].color = Color.black;
        }
        TempText.Clear();
        SavedText.Clear();
        index = 0;
    }

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        UIManager.Instance.SelectedBlock = this;
        for (int i = 0; i < TempText.Count; i++) {
            TempText[i].text = SavedText[i];
            TempText[i].color = Color.black;
        }
        TempText.Clear();
        SavedText.Clear();
        index = 0;
        if (UIManager.Instance.IsDraggingItem &&
                    SelectedItem.Type != ItemTypes.Material && SelectedItem.Type != ItemTypes.Key &&
                    Equippable(SelectedItem)) {
            //old stats -> StatChange
            if (SelectedItem.Type == ItemTypes.Consumable && Character.GetComponent<Equipment>())
                Character.GetComponent<Equipment>().Consume(SelectedItem);
            else
                Character.GetComponent<Equipment>().Equip(SelectedItem);
            //deselect item and show character details
            UIManager.Instance.DeselectItem();
            SelectCharacter();
        }

    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        SelectCharacter();
        UIManager.Instance.SelectedBlock = this;
    }

    public void SelectCharacter()
    {
        UIManager.Instance.MainMenuDetailsImage.sprite = MyStats.BigSprite;
        UIManager.Instance.SelectedCharacter = Character;
        UIManager.Instance.TossItemButton.SetActive(false);
        //update equipment list display
        if (MyEquip.Head)
            UIManager.Instance.EquipListTexts[0].text = MyEquip.Head.Name;
        else
            UIManager.Instance.EquipListTexts[0].text = "Empty";
        if (MyEquip.Hand)
            UIManager.Instance.EquipListTexts[1].text = MyEquip.Hand.Name;
        else
            UIManager.Instance.EquipListTexts[1].text = "Empty";
        if (MyEquip.Torso)
            UIManager.Instance.EquipListTexts[2].text = MyEquip.Torso.Name;
        else
            UIManager.Instance.EquipListTexts[2].text = "Empty";
        if (MyEquip.Legs)
            UIManager.Instance.EquipListTexts[3].text = MyEquip.Legs.Name;
        else
            UIManager.Instance.EquipListTexts[3].text = "Empty";
        if (MyEquip.Extra)
            UIManager.Instance.EquipListTexts[4].text = MyEquip.Extra.Name;
        else
            UIManager.Instance.EquipListTexts[4].text = "Empty";
        UIManager.Instance.EquipList.SetActive(true);
    }
}