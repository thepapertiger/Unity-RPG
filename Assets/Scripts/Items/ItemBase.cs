/* NAME:            ItemBase.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     Item class for every item in game. It is a Scriptable Object.
 * REQUIREMENTS:    Values must be filled out in Unity Editor. Default value for
 *                  integer fields is 0. Currently, only Gold Value
 *                  can be set publicly.
 *                  Please use the case conventions of 'Title Case'. First and Last words
 *                  are always capitalized and every other word except articles (of, the, a).
 *                  Search "Title Case Converter" online, if in doubt.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTypes { Consumable, Weapon, Gear, Material, Key };

public class ItemBase : ScriptableObject {
    
    public Sprite Sprite;

    /// <summary>
    /// Name of item
    /// </summary>
    [SerializeField]
    private string _Name = "<Use the Convention of 'Title Case'>";
    public string Name {
        get
        {
            return _Name;
        }
    }

    /// <summary>
    /// Type of item (Consumable, Weapon, Gear, Material, Key)
    /// </summary>
    [SerializeField]
    private ItemTypes _Type;
    public ItemTypes Type {
        get
        {
            return _Type;
        }
    }

    /// <summary>
    /// Maximum amount of this item that player can carry at any given time.
    /// </summary>
    [SerializeField, Range(1, 999)]
    private int _MaxAmount = 1;
    public int MaxAmount {
        get
        {
            return _MaxAmount;
        }
        set //MaxAmount may incraese with bag upgrades
        {
            if (value < 1)
               _MaxAmount = 1;
            else if (value > 999)
                _MaxAmount = 999;
            else
                _MaxAmount = value;
        }
    }

    /// <summary>
    /// Gold value of item. Limit is one million chips.
    /// </summary>
    [SerializeField, Range(0, 1000000000)]
    private int _GoldValue = 0;
    public int GoldValue {
        get
        {
            return _GoldValue;
        }
        set //inflation may occur as player progresses
        {
            if (value < 0)
                _GoldValue = 0;
            else if (value > 1000000000)
                _GoldValue = 1000000000;
            else
                _GoldValue = value;
        }
    }

    /// <summary>
    /// The amount of HP this item replenishes.
    /// </summary>
    [SerializeField, Range(0, 1000000000)]
    private int _HPRecovery = 0;
    public int HPRecovery {
        get
        {
            return _HPRecovery;
        }
    }

    /// <summary>
    /// The amount of MP this item replenishes.
    /// </summary>
    [SerializeField, Range(0, 1000000000)]
    private int _MPRecovery = 0;
    public int MPRecovery {
        get
        {
            return _MPRecovery;
        }
    }

    /// <summary>
    /// The attack Value points added to the player when wielded.
    /// </summary>
    [SerializeField, Range(-1000, 1000)]
    private int _AttackValue = 0;
    public int AttackValue {
        get
        {
            return _AttackValue;
        }
    }

    /// <summary>
    /// The Defense Value points added to the player when wielded.
    /// </summary>
    [SerializeField, Range(-1000, 1000)]
    private int _DefenseValue = 0;
    public int DefenseValue {
        get
        {
            return _DefenseValue;
        }
    }

    /// <summary>
    /// The Speed Value points added to the player when wielded.
    /// </summary>
    [SerializeField, Range(-1000, 1000)]
    private int _AgilityValue = 0;
    public int AgilityValue {
        get
        {
            return _AgilityValue;
        }
    }

    /// <summary>
    /// Description of item
    /// </summary>
    [SerializeField, Multiline]
    private string _Description = "";
    public string Description {
        get
        {
            return _Description;
        }
    }
}