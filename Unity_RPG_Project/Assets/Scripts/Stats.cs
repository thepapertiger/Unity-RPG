using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour {

	public string Name;
	public int Level;
	public int MaxHP;
	public int HP;
	public int MaxMP;
	public int MP;
	public int AttackDamage;

	public Stats()
	{
		Name = "Player";
		Level = 1;
		MaxHP = 100;
		HP = 100;
		MaxMP = 100;
		MP = 100;
		AttackDamage = 10;
	}

	public Stats(string name, int level, int max_hp, int hp, int max_mp, int mp, int atk_dmg)
	{
		Name = name;
		Level = level;
		MaxHP = max_hp;
		HP = hp;
		MaxMP = max_mp;
		MP = mp;
		AttackDamage = atk_dmg;
	}

	void Attack(Stats target)
	{
		target.HP -= AttackDamage;
		if (target.HP < 0)
			target.HP = 0;
	}
}
