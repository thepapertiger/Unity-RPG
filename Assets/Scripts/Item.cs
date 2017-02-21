/* NAME:            Item.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     Base class for every item in game.
 * REQUIREMENTS:    Name, Type, Description, Gold Value
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Item  {

    /// <summary>
    /// Name of item
    /// </summary>
    [SerializeField]
    private string _Name;
    public string Name
    {
        get
        {
            return _Name;
        }
        set
        {
            _Name = value;
        }
    }

    /// <summary>
    /// Type of item (Consumable, Weapon, Gear, Material, Key)
    /// </summary>
    [SerializeField]
    private string _Type;
    public string Type
    {
        get
        {
            return _Type;
        }
        set
        {
            _Type = value;
        }
    }

    /// <summary>
    /// Description of item
    /// </summary>
    [SerializeField]
    private string _Description;
    public string Description
    {
        get
        {
            return _Description;
        }
        set
        {
            _Description = value;
        }
    }

    /// <summary>
    /// Gold value of item.
    /// </summary>
    [SerializeField]
    private float _GoldValue;
    public float GoldValue
    {
        get
        {
            return _GoldValue;
        }
        set
        {
            _GoldValue = value;
        }
    }
}
