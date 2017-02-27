using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuPartyBlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public Stats MyStats;

    private List<Text> TempText = new List<Text>(); //references to all Text changed due to item hover
    private List<string> SavedText = new List<string>(); //saved text of TempText list; revert on end drag
    private int index = 0;
    private ItemBase SelectedItem;
    private int StatChange;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        SelectedItem = UIManager.Instance.SelectedItem; //shorten code

//TODO: check if item is compatible with this character #################
//TODO: subract stat buff from removed/swapped equipment

        if (UIManager.Instance.IsDraggingItem && 
                    SelectedItem.Type != ItemTypes.Material && SelectedItem.Type != ItemTypes.Key) {
            switch (SelectedItem.Type) {
                case (ItemTypes.Consumable):
                    if (MyStats.HP < MyStats.MaxHP && SelectedItem.HPRecovery > 0) { //check if theres a change
                        UpdateStatDisplay("HP", MyStats.HP, SelectedItem.HPRecovery, MyStats.MaxHP);
                    }
                    if (MyStats.MP < MyStats.MaxMP && SelectedItem.MPRecovery > 0) {
                        print(MyStats.MP + ", " + MyStats.MaxMP + ", " + SelectedItem.MPRecovery);
                        UpdateStatDisplay("MP", MyStats.MP, SelectedItem.MPRecovery, MyStats.MaxMP);
                        }
                        break;
                case (ItemTypes.Weapon):
                    if (SelectedItem.AttackValue != 0)
                        UpdateStatDisplay("STR", MyStats.Strength, SelectedItem.AttackValue);
                    if (SelectedItem.AgilityValue != 0)
                        UpdateStatDisplay("AGL", MyStats.Agility, SelectedItem.AgilityValue);
                    break;
                case (ItemTypes.Gear):
                    if (SelectedItem.DefenseValue != 0)
                        UpdateStatDisplay("DEF", MyStats.Defense, SelectedItem.DefenseValue);
                    if (SelectedItem.AgilityValue != 0)
                        UpdateStatDisplay("AGL", MyStats.Agility, SelectedItem.AgilityValue);
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
        print(index);
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
        for (int i = 0; i < TempText.Count; i++) {
            TempText[i].text = SavedText[i];
            TempText[i].color = Color.black;
        }
        TempText.Clear();
        SavedText.Clear();
        index = 0;

        //TODO: EQUIP/SWAP ITEM
        //old stats -> StatChange
        //subtract from inventory if used
        //if slider update: Player.Instance.UpdateParty();

    }

}
