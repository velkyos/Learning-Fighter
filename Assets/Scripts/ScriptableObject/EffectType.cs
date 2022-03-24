using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Effect
{
    public EffectEnum effectType;
    public int value;

    public Effect(EffectEnum effectType, int value)
    {
        this.effectType = effectType;
        this.value = value;
    }
}


[System.Serializable]
public enum EffectEnum
{
    Health,
    Agility,
    MagicProtection,
    PhysicProtection,
    Mana,
    Stamina
}