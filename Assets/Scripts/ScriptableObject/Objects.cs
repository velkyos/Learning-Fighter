using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CS_Object
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class Weapon : CS_Object
{
    public int weight;
    public int attackDamage;

    public Weapon(int weight, int attackDamage)
    {
        this.weight = weight;
        this.attackDamage = attackDamage;
        name = "Weapon";
    }
}

[System.Serializable]
public class Spell : CS_Object
{
    public int energyCost;
    public Effect effect;

    public Spell(int energyCost, Effect effect, bool passive)
    {
        this.energyCost = energyCost;
        this.effect = effect;
        name = "Spell";
    }
}

[System.Serializable]
public class Armor : CS_Object
{
    public int weight;
    public int physicProtection;
    public int magicalProtection;

    public Armor(int weight, int physicProtection, int magicalProtection)
    {
        this.weight = weight;
        this.physicProtection = physicProtection;
        this.magicalProtection = magicalProtection;
        name = "Armor";
    }
}

[System.Serializable]
public class Potion : CS_Object
{
    public Effect effect;
    public int numbers;

    public Potion(Effect effect, int numbers)
    {
        this.effect = effect;
        this.numbers = numbers;
        name = "Potion";
    }
}

[System.Serializable]
public class Inventory
{
    public string name;
    public Weapon weapon;

    public Armor armor;

    public Spell spell;

    public Potion potion;
}

[System.Serializable]
public enum ActionEnum
{
    Start = 0,
    None = 1,
    Weapon = 2,
    Spell = 3,
    Potion = 4,
    Dodge = 5
}