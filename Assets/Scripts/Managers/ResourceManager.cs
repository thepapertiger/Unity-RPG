/* NAME:            MovingObject.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     The Resource Manager is meant to keep references of all assets like item
 *                  scriptable objects and Sounds that will need to be accessed. That way,
 *                  everything can on be loaded once in the beginning of the game instead of
 *                  having wait times throughout the game.
 * REQUIREMENTS:    Singleton class must be defined with a static "Instance" variable.
 *                  To avoid typos, item naming convention should mimic book-titles; have each 
 *                  word separate and each word starts with a capital letter except for 
 *                  articles (of, the, a, in). Example: "Oil of a Golem".
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : Singleton<ResourceManager> {
    //the Dictionaries that keep each reference
    private Dictionary<string, ItemBase> ItemDict;
    private Dictionary<string, AudioClip> SoundDict;

    protected override void Awake()
    {
        base.Awake();
        ItemDict = new Dictionary<string, ItemBase>();
        SoundDict = new Dictionary<string, AudioClip>();

        LoadItems();
        LoadSounds();
    }

    /// <summary>
    /// Loads item from the resources folder
    /// </summary>
    private void LoadItems()
    {
        object[] loaded_items = Resources.LoadAll("Items");
        foreach (ItemBase i in loaded_items) {
            if (!ItemDict.ContainsKey(i.Name)) {
                ItemDict.Add(i.Name, i);
            }
        }
    }

    /// <summary>
    /// Loads the songs from the Resources folder
    /// </summary>
    private void LoadSounds()
    {
       
        object[] loaded_items = Resources.LoadAll("Sounds");
        foreach ( AudioClip i in loaded_items) {
            if (!SoundDict.ContainsKey(i.name)) {
                SoundDict.Add(i.name, i);
            }
        }
    }

    public ItemBase GetItem(string name)
    {
        if (ItemDict.ContainsKey(name)) {
            return ItemDict[name];
        }
        else {
            Debug.LogError(name + " not found, it may be mispelled.");
            return null;
        }
    }

    public AudioClip GetSound(string name)
    {
        if (SoundDict.ContainsKey(name)) {
            return SoundDict[name];
        }
        else {
            Debug.LogError(name + " not found, it may be mispelled.");
            return null;
        }
    }
}
