/* NAME:            Stats.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION: 	This is the Stats script. It holds stats relevant to combat
 * 					such as HP, MP, AttackDamage, etc. 
 * REQUIREMENTS:    None
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
	//Bar Stats
    public string Name;
    public bool Playable;
    public int Level;
    public int MaxHP;
    public int HP;
    public int MaxMP;
    public int MP;
	public int EXP;
	public int EXPNeeded;
	//Skill Stats
    public int Strength;
    public int Agility;
    public int Intelligence;
    public int Defense;
    public int Dexterity;
    public int Luck;
    public bool defending = false;
    
    public Sprite SmallSprite;
    public Sprite BigSprite;

    public Stats()
    {
        Name = "Player";
        Playable = true;
        Level = 1;
        MaxHP = 100;
        HP = 100;
        MaxMP = 100;
        MP = 100;
		Strength = 10;
        Agility = 10;
        Intelligence = 10;
        Defense = 10;
        Dexterity = 10;
        Luck = 10;
    }

    public Stats(string name, bool play, int level, int max_hp, int max_mp, int str, int agl, int intl, int def, int dex, int lck)
    {
        Name = name;
        Playable = play;
        Level = level;
        MaxHP = max_hp;
        HP = max_hp;
        MaxMP = max_mp;
        MP = max_mp;
		Strength = str;
        Agility = agl;
        Intelligence = intl;
        Defense = def;
        Dexterity = dex;
        Luck = lck;
    }

    ///<summary>
    ///Attack: Subtracts attack damage from target's HP
    ///<summary>
    public int Attack(Stats target)
    {
		if (HP <= 0)
			return 0;
        //calculate special damage here
		int atkDmg = Random.Range(Strength / 2, Strength);  //varies the damage
        return target.TakeDamage(atkDmg);
    }

    ///<summary>
    ///Designate an amount of damage to be taken by the stat. Returns the damage dealt after calculation
    ///<summary>
    public int TakeDamage(int dmg)
    {
        if (defending) {
			dmg -= Defense;    //small flat damage reduction
            if (dmg < 0)
                dmg = 0;
            dmg = dmg / 2;
        }
        HP -= dmg;
        if (HP < 0)
            HP = 0;
        return dmg;
    }
}
