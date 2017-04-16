using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    private Stats MyStats;

    //Equipment slots
    public ItemBase Head;
    public ItemBase Hand;
    public ItemBase Torso;
    public ItemBase Legs;
    public ItemBase Extra;

    private void Start()
    {

        if (!MyStats)
            MyStats = gameObject.GetComponent<Stats>();
    }

    /// <summary>
    /// Consume items that restore HP/MP
    /// </summary>
    public void Consume(ItemBase consumable)
    {
        if (MyStats.HP < MyStats.MaxHP && consumable.HPRecovery > 0) { //check if theres a change
            MyStats.HP += consumable.HPRecovery;
            if (MyStats.HP > MyStats.MaxHP)
                MyStats.HP = MyStats.MaxHP;
            Inventory.Instance.RemoveItem(consumable.Name, 1); //consume
            Player.Instance.UpdateParty(); //update sliders
        }
        if (MyStats.MP < MyStats.MaxMP && consumable.MPRecovery > 0) {
            MyStats.MP += consumable.MPRecovery;
            if (MyStats.MP > MyStats.MaxMP)
                MyStats.MP = MyStats.MaxMP;
            Inventory.Instance.RemoveItem(consumable.Name, 1); //consume
            Player.Instance.UpdateParty(); //update sliders
        }
    }

    /// <summary>
    /// Equip a new item, swap with old item in that slot if necessary.
    /// </summary>
    public void Equip(ItemBase new_item)
    {
        //Increase stats
        MyStats.Strength += new_item.AttackValue;
        MyStats.Defense += new_item.AgilityValue;
        MyStats.Agility += new_item.AgilityValue;

        //check if already item in slot, return to inventory
        switch (new_item.EquipSlot) {
            case EquipSlots.None:
                break;
            case EquipSlots.Head:
                if (Head != null) { //if there is already an item
                    Inventory.Instance.AddItem(Head.Name, 1); //return it to inventory
                    LowerStats(gameObject.GetComponent<Stats>(), Head); //reduce stat boost
                }
                Head = new_item; //set new item
                break;
            case EquipSlots.Hand:
                if (Hand != null) { //if there is already an item
                    Inventory.Instance.AddItem(Hand.Name, 1); //return it to inventory
                    LowerStats(gameObject.GetComponent<Stats>(), Hand); //reduce stat boost
                }
                Hand = new_item; //set new item
                break;
            case EquipSlots.Torso:
                if (Torso != null) {
                    Inventory.Instance.AddItem(Torso.Name, 1);
                    LowerStats(gameObject.GetComponent<Stats>(), Torso); //reduce stat boost
                }
                Torso = new_item;
                break;
            case EquipSlots.Legs:
                if (Legs != null) {
                    Inventory.Instance.AddItem(Legs.Name, 1);
                    LowerStats(gameObject.GetComponent<Stats>(), Legs); //reduce stat boost
                }
                Legs = new_item;
                break;
            case EquipSlots.Extra:
                if (Extra != null) {
                    Inventory.Instance.AddItem(Extra.Name, 1);
                    LowerStats(gameObject.GetComponent<Stats>(), Extra); //reduce stat boost
                }
                Extra = new_item;
                break;
        }
        //remove from inventory
        Inventory.Instance.RemoveItem(new_item.Name, 1);
        Player.Instance.UpdateParty();
    }

    /// <summary>
    /// Called by Unequip buttons to Unequip item in specified static slot
    /// </summary>
    public void Unequip(EquipSlots slot)
    {
        ItemBase item = UIManager.Instance.SelectedCharacter.GetComponent<Equipment>().GetEquipSlot(slot);
        if (item) {
            Inventory.Instance.AddItem(item.Name, 1);
            switch (slot) {
                case EquipSlots.Head:
                    Head = null;
                    break;
                case EquipSlots.Hand:
                    Hand = null;
                    break;
                case EquipSlots.Torso:
                    Torso = null;
                    break;
                case EquipSlots.Legs:
                    Legs = null;
                    break;
                case EquipSlots.Extra:
                    Extra = null;
                    break;
            }
        }
    }
    
    /// <summary>
    /// Returns the magnitude of the stat reduction if this item is to be equipped.
    /// Based on the EquipSlot of the new item, the old item is checked.
    /// </summary>
    public ItemBase GetEquipSlot(EquipSlots slot)
    {
        switch (slot) {
            case EquipSlots.Head:
                return Head;
            case EquipSlots.Hand:
                return Hand;
            case EquipSlots.Torso:
                return Torso;
            case EquipSlots.Legs:
                return Legs;
            case EquipSlots.Extra:
                return Extra;
            default:
                return null;
        }
    }

    /// <summary>
    /// Lowers all stats that were boosted by the item which is now removed
    /// </summary>
    private void LowerStats(Stats stats, ItemBase item_removed)
    {
        stats.Strength -= item_removed.AttackValue;
        stats.Defense -= item_removed.DefenseValue;
        stats.Agility -= item_removed.AgilityValue;
    }
}
