using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : IComparable<Player>
{
    #region Variable

    public int id = -1;
    public int gen = -1;

    public NeuronNetwork brain = null;

    //Inventory

    public Inventory inventory;
    public int inventoriesNbr;
    public int inventoryIndex;
    public int potionNbr;

    //Current Stats

    public int health;
    public int agility;

    public int magicalProtection;
    public int mana;
    public int physicProtection;
    public int stamina;

    public int totalWeight;

    //Game Stats

    public Player enemy;

    public ActionEnum lastAction;
    public ActionEnum currentAction;
    public bool lastActionUsefull;

    public int score;      //Score during the fight

    public bool win;
    public bool isDead;

    //Tournament Mode
    public int tSumDif;         //Score used to Compare between player with the same victoryNbr
    public int tvictoryPoints;
    public int totalFight;

    #endregion

    #region Initialization
    public Player(int newId, NeuronNetwork newBrain)
    {
        id = newId;
        gen = 1;
        brain = newBrain;
    }

    public void NewGeneration(Inventory newInventory,int newInventoryIndex, int inventoriesNbr)
    {
        this.inventory = newInventory;
        this.inventoryIndex = newInventoryIndex;
        this.inventoriesNbr = inventoriesNbr;
        tSumDif = 0;
        tvictoryPoints = 0;
        totalFight = 0;
    }

    public void NewFight(Player newEnemy)
    {
        totalFight++;
        enemy = newEnemy;

        lastActionUsefull = false;
        lastAction = ActionEnum.Start;
        currentAction = ActionEnum.Start;
        score = 0;
        win = false;
        isDead = false;
        //Debug.LogError(id + " / " + enemy.id);
        UpdateStats();
    }

    private void UpdateStats()
    {
        health = Stats.BASE_STAT;
        agility = Stats.BASE_STAT;

        mana = Stats.BASE_STAT;
        stamina = Stats.BASE_STAT;

        magicalProtection = inventory.armor.magicalProtection;
        physicProtection = inventory.armor.physicProtection;

        totalWeight = inventory.armor.weight + inventory.weapon.weight;

        agility += (int)(totalWeight * Stats.AGILITY_COST);

        potionNbr = inventory.potion.numbers;
    }
    #endregion

    #region Finish
    public void EndFight()
    {
        score += (health - enemy.health) * 4;

        if (win)
            tvictoryPoints += 3;
        else if (!win && !enemy.win)
            tvictoryPoints += 1;

        if (score > enemy.score)
            tvictoryPoints += 2;

        tSumDif += (score - enemy.score);
    }

    #endregion

    #region Brain
    public List<float> InitInput()
    {
        List<float> inputs = new List<float>();
        inputs.Add(health / Stats.MAX_EFFECT_VALUE);
        inputs.Add(agility / Stats.MAX_EFFECT_VALUE);
        inputs.Add(mana / Stats.MAX_EFFECT_VALUE);
        inputs.Add(stamina / Stats.MAX_EFFECT_VALUE);
        inputs.Add(inventoryIndex / inventoriesNbr);
        inputs.Add(potionNbr / Stats.MAX_POTION);
        inputs.Add((int)lastAction / 5f);
        return inputs;
    }

    public void chooseAction(float ratioRound)
    {
        lastAction = currentAction;
        float lastActionUsefullValue = lastActionUsefull ? 1f : 0f;

        float[] output;
        List<float> input = InitInput();
        input.AddRange(enemy.InitInput());
        input.Add(ratioRound);

        input.Add(lastActionUsefullValue);
        output = brain.FeedForward(input.ToArray());

        float valeurMax = float.MinValue;
        int max = 5;
        for (int i = 0; i < output.Length; i++)
        {
            if (output[i] >= valeurMax)
            {
                valeurMax = output[i];
                max = i;
            }
        }
        lastActionUsefull = false;
        currentAction = (ActionEnum)(max + 1);
    }

    #endregion

    #region inFight
    public void DoAction()
    {
        
        switch (currentAction)
        {
            case ActionEnum.Weapon:
                UseWeapons();
                break;
            case ActionEnum.Spell:
                UseSpell();
                break;
            case ActionEnum.Potion:
                UsePotion();
                break;
            case ActionEnum.Dodge:
                UseDodge();
                break;
            default:
                break;
        }
    }

    private void UseDodge()
    {
        if (enemy.currentAction != ActionEnum.Weapon)
        {
            score -= 50;
        }
        else
        {
            lastActionUsefull = true;
        }
    }
    private void UseWeapons()
    {
        int staminaCost = (int)( (inventory.weapon.weight) * Stats.STAMINA_COST);
        bool agilityTest = UnityEngine.Random.Range(0, 100) <= agility;

        float agilityRatio = (enemy.agility - agility + 100) / 20;
        /*
         *  Dotge Test work with a ration from 0 to 10,
         *  < 1 mean you have more than 5 times the agility of your enemy
         *  5 mean you have the same agility of your enemy
         *  > 9 mean your enemy have more than 5 times your agility
        */
        bool dodgeTest = UnityEngine.Random.Range(0f, 10f) <= agilityRatio;

        //Verify if we can use the action.
        if (stamina >= staminaCost)
        {
            //We remove the stamina
            stamina -= staminaCost;
            //Agility Test
            if (agilityTest)
            {
                //Verify if the enemy has dodge
                if (dodgeTest && enemy.currentAction == ActionEnum.Dodge)
                {
                    score += 30;
                    enemy.score += 100;
                }
                else if (!dodgeTest && enemy.currentAction == ActionEnum.Dodge)
                {
                    enemy.score += 20;
                    enemy.ApplyPhysicDamage(inventory.weapon.attackDamage);
                    score += 100;
                }
                else
                {
                    score += 100;
                    enemy.ApplyPhysicDamage(inventory.weapon.attackDamage);
                    
                }
                lastActionUsefull = true;
                
            }
            else
                score += 20;
        }
        else
            score -= 50;
    }

    private void UsePotion()
    {
        if (potionNbr > 0)
        {
            potionNbr--;

            bool result = ChangeStatsWithMagic(inventory.potion.effect.effectType, inventory.potion.effect.value, false);
            if (result)
            {
                score += 100;
                lastActionUsefull = true;
            }
            else
                score -= 20;
        }
        else
            score -= 50;
    }

    private void UseSpell()
    {
        if (mana >= inventory.spell.energyCost)
        {
            mana -= inventory.spell.energyCost;

            bool result = enemy.ChangeStatsWithMagic(inventory.spell.effect.effectType, inventory.spell.effect.value, true);
            if (result)
            {
                score += 100;
                lastActionUsefull = true;
            }

            else
                score -= 20;
        }
        else
            score -= 50;
    }
    #endregion

    #region Change Stats
    public bool ChangeStatsWithMagic(EffectEnum effect, int value, bool byEnemy)
    {
        /*  To test if an action is usefull we need
         *      realValue to be more than 0. ( Player understand the magicalProtection )
         *      or
         *      The target stat of the potion is not already full
         */
        bool isUsefull = false;

        int realValue = value;

        if (byEnemy)
            realValue = (-1)*onlyBetween(value - magicalProtection, (int)(0.2f * value), 100);

        if (realValue != 0)
        {
            switch (effect)
            {
                case EffectEnum.Health:

                    isUsefull = !( (!byEnemy) && (health == Stats.BASE_STAT) );

                    health = onlyBetween(health + realValue, 0, Stats.BASE_STAT);
                    isDead = health == 0;
                    break;
                case EffectEnum.Agility:

                    isUsefull = !((!byEnemy) && (agility == Stats.BASE_STAT));

                    agility = onlyBetween(agility + realValue, 0, Stats.BASE_STAT);
                    break;
                case EffectEnum.MagicProtection:

                    isUsefull = !((!byEnemy) && (magicalProtection == Stats.BASE_STAT));

                    magicalProtection = onlyBetween(magicalProtection + realValue, 0, Stats.BASE_STAT);
                    break;
                case EffectEnum.PhysicProtection:

                    isUsefull = !((!byEnemy) && (physicProtection == Stats.BASE_STAT));

                    physicProtection = onlyBetween(physicProtection + realValue, 0, Stats.BASE_STAT);
                    break;
                case EffectEnum.Mana:

                    isUsefull = !((!byEnemy) && (mana == Stats.BASE_STAT));

                    mana = onlyBetween(mana + realValue, 0, Stats.BASE_STAT);
                    break;
                case EffectEnum.Stamina:

                    isUsefull = !((!byEnemy) && (stamina == Stats.BASE_STAT));

                    stamina = onlyBetween(stamina + realValue, 0, Stats.BASE_STAT);
                    break;
                default:
                    break;
            }
        }
        return isUsefull;
    }

    public void ApplyPhysicDamage(int damage)
    {
        int realValue = onlyBetween(damage - physicProtection, (int)(0.2f * damage), Stats.BASE_STAT);

        health = onlyBetween(health - realValue, 0, Stats.BASE_STAT);
        isDead = health == 0;
    }
    #endregion

    #region Utils
    //Return Min if x < Min and Max if x > Max
    private int onlyBetween(int x, int min, int max)
    {
        x = x < min ? min : x;
        x = x > max ? max : x;
        return x;
    }

    public Player Copy(Player p, int newId, int newGen)
    {
        p.brain = this.brain.Copy(p.brain);
        p.gen = newGen;
        p.inventory = this.inventory;
        p.id = newId;
        return p;
    }

    public int CompareTo(Player other)
    {
        if (other == null) return 1;

        if (tvictoryPoints > other.tvictoryPoints)
            return 1;
        else if (tvictoryPoints < other.tvictoryPoints)
            return -1;
        else
        {
            if (tSumDif > other.tSumDif)
                return 1;
            else if (tSumDif < other.tSumDif)
                return -1;
            else
                return 0;
        }
    }

    public static string GetStringFromEffect(EffectEnum effect)
    {
        string val = "";
        switch (effect)
        {
            case EffectEnum.Health:
                val = "Health";
                break;
            case EffectEnum.Agility:
                val = "Agility";
                break;
            case EffectEnum.MagicProtection:
                val = "M.Protection";
                break;
            case EffectEnum.PhysicProtection:
                val = "P.Protection";
                break;
            case EffectEnum.Mana:
                val = "Mana";
                break;
            case EffectEnum.Stamina:
                val = "Stamina";
                break;
            default:
                break;
        }
        return val;
    }
    #endregion
}



