using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public struct PlayerStruct
{
    [Range(0, 100)] public int health;
    [Range(0, 100)] public int agility;
    [Range(0, 100)] public int mana;
    [Range(0, 100)] public int magicalProtection;
    [Range(0, 100)] public int stamina;
    [Range(0, 100)] public int physicProtection;
    public ActionEnum action;
    [Range(0, 100)]public int attackDamage;
    public EffectEnum potionEffect;
    [Range(0, 100)] public int potionValue;
    [Range(0, 100)] public int potionNumber;
    public EffectEnum spellEffect;
    [Range(0, 100)] public int spellValue;
    [Range(0, 100)] public int spellCost;
     public bool spellPasive;
}

public class InputLayer : MonoBehaviour
{
    [HideInInspector]
    public static readonly float MAX_POTION = 5f;
    [HideInInspector]
    public static readonly float MAX_EFFECT_VALUE = 100f;
    [HideInInspector]
    public static readonly float NBR_EFFECT = 6f;
    [HideInInspector]
    public static readonly float MAX_ENERGY_COST = 100f;

    public float[] inputs = new float[31];

    [Range(1, 25)] public int RoundNumber;
    [SerializeField]
    public PlayerStruct playerA;
    [SerializeField]
    public PlayerStruct playerB;

    public VisualisationManager manager;

    public void UpdateChange()
    {
        List<float> newInputs = new List<float>();
        newInputs.AddRange(InitInput(playerA));
        newInputs.AddRange(InitInput(playerB));
        newInputs.Add(RoundNumber / 25f);
        inputs = newInputs.ToArray();

        manager.UpdateGaphic(inputs);
    }

    private List<float> InitInput(PlayerStruct player)
    {
        int lastAction = 4, potionType = 0, spellType = 0;
        switch (player.action)
        {
            case ActionEnum.Weapon:
                lastAction = 0;
                break;
            case ActionEnum.Spell:
                lastAction = 1;
                break;
            case ActionEnum.Potion:
                lastAction = 2;
                break;
            case ActionEnum.Dodge:
                lastAction = 3;
                break;
            default:
                break;
        }
        switch (player.spellEffect)
        {
            case EffectEnum.Health:
                spellType = 1;
                break;
            case EffectEnum.Agility:
                spellType = 6;
                break;
            case EffectEnum.MagicProtection:
                spellType = 5;
                break;
            case EffectEnum.PhysicProtection:
                spellType = 3;
                break;
            case EffectEnum.Mana:
                spellType = 4;
                break;
            case EffectEnum.Stamina:
                spellType = 2;
                break;
            default:
                break;
        }
        switch (player.potionEffect)
        {
            case EffectEnum.Health:
                potionType = 1;
                break;
            case EffectEnum.Agility:
                potionType = 6;
                break;
            case EffectEnum.MagicProtection:
                potionType = 5;
                break;
            case EffectEnum.PhysicProtection:
                potionType = 3;
                break;
            case EffectEnum.Mana:
                potionType = 4;
                break;
            case EffectEnum.Stamina:
                potionType = 2;
                break;
            default:
                break;
        }


        List<float> input = new List<float>();
        input.Add(player.health / MAX_EFFECT_VALUE);
        input.Add(player.mana / MAX_EFFECT_VALUE);
        input.Add(player.magicalProtection / MAX_EFFECT_VALUE);
        input.Add(player.stamina / MAX_EFFECT_VALUE);
        input.Add(player.physicProtection / MAX_EFFECT_VALUE);
        input.Add(player.agility / MAX_EFFECT_VALUE);
        input.Add(lastAction / 4f);
        input.Add(player.attackDamage / MAX_EFFECT_VALUE);
        input.Add(potionType / MAX_EFFECT_VALUE);
        input.Add(player.potionValue / MAX_EFFECT_VALUE);
        input.Add(player.potionNumber / MAX_POTION);
        input.Add(spellType / MAX_EFFECT_VALUE);
        input.Add(player.spellValue / MAX_EFFECT_VALUE);
        input.Add(player.spellCost / MAX_ENERGY_COST);
        if (player.spellPasive)
        {
            input.Add(1);
        }
        else
        {
            input.Add(0);
        }
        return input;
    }

}
