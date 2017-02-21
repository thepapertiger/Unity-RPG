/* NAME:            Inventory.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     This manages items stored in a list for the player to add/remove items, limit by number, display quantity/sprite of items.
 * REQUIREMENTS:    None
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    /// <summary>
    /// Maximum number of items in inventory
    /// </summary>
    private int MaxCapacity;

    /// <summary>
    /// All player's items
    /// </summary>
    public List<Item> Gauntlet = new List<Item>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //TODO: Implement maximum amount of a type of item 

    /// <summary>
    /// Add item in player's inventory
    /// </summary>
    /// <param name="item"></param>
    private void AddItem(Item item)
    {
        if(Gauntlet.Count == MaxCapacity)
        {
            Debug.Log("Bag full.");
            return;
        }
        Gauntlet.Add(item);
    }

    /// <summary>
    /// Remove item from player inventory
    /// </summary>
    /// <param name="item"></param>
    private void RemoveItem(Item item)
    {
        if(!HasItem(item))
        {
            Debug.Log("Item does not exist in inventory.");
            return;
        }
        Gauntlet.Remove(item);
    }

    /// <summary>
    /// Checks if Player owns item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private bool HasItem(Item item)
    {
        return Gauntlet.Contains(item);
    }
}
