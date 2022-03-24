using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_UI : MonoBehaviour
{

    [System.Serializable]
    public struct Cs_slider{
        
        public Slider slider;
        public TextMeshProUGUI text;
    }

    [System.Serializable]
    public struct Cs_Icon
    {
        public Image icon;
        public TextMeshProUGUI name;
    }

    public bool played;

    public Cs_Icon[] icons;
    public Cs_slider[] stats;
    public TextMeshProUGUI id_text;
    public TextMeshProUGUI current_text;
    public TextMeshProUGUI last_text;
    public Player currentPlayer;


    private int[] maxStats;

    public void updatePlayer()
    {
        if (!played)
            id_text.text = "# " + (currentPlayer.id) + " / Gen " + (currentPlayer.gen);

        updateIcon(icons[0], currentPlayer.inventory.armor);
        updateIcon(icons[1], currentPlayer.inventory.weapon);
        updateIcon(icons[2], currentPlayer.inventory.potion, currentPlayer.potionNbr);
        updateIcon(icons[3], currentPlayer.inventory.spell);
        updateSlider(stats[0], currentPlayer.health, maxStats[0]);
        updateSlider(stats[1], currentPlayer.agility, maxStats[1]);
        updateSlider(stats[2], currentPlayer.physicProtection, maxStats[2]);
        updateSlider(stats[3], currentPlayer.stamina, maxStats[3]);
        updateSlider(stats[4], currentPlayer.magicalProtection, maxStats[4]);
        updateSlider(stats[5], currentPlayer.mana, maxStats[5]);
        updateActionText(last_text, currentPlayer.lastAction);
        updateActionText(current_text, currentPlayer.currentAction);
    }

    private void updateIcon(Cs_Icon cs_Icon, Armor cs_object)
    {
        cs_Icon.name.text = cs_object.name + " (" + cs_object.weight + "kg)" + "\n" + "Physic : " + cs_object.physicProtection + "\nMagic : " + cs_object.magicalProtection;
        cs_Icon.icon.sprite = cs_object.icon;
    }

    private void updateIcon(Cs_Icon cs_Icon, Weapon cs_object)
    {
        cs_Icon.name.text = cs_object.name + " (" + cs_object.weight + "kg)" + "\n"  + "Damage : " + cs_object.attackDamage;
        cs_Icon.icon.sprite = cs_object.icon;
    }

    private void updateIcon(Cs_Icon cs_Icon, Potion cs_object, int potionNbr)
    {
        string type = Player.GetStringFromEffect(cs_object.effect.effectType);
        cs_Icon.name.text = potionNbr + "x " + cs_object.name + "\n(" + "+" + cs_object.effect.value + " " + type +")";
        cs_Icon.icon.sprite = cs_object.icon;
    }

    private void updateIcon(Cs_Icon cs_Icon, Spell cs_object)
    {
        string type = Player.GetStringFromEffect(cs_object.effect.effectType);
        cs_Icon.name.text = cs_object.name + "\n (-" + cs_object.effect.value + " " + type + " / " +  cs_object.energyCost + " Mana)";
        cs_Icon.icon.sprite = cs_object.icon;
    }

    private void updateActionText(TextMeshProUGUI textUI, ActionEnum action)
    {
        string text = "";
        text = action.ToString();
        textUI.text = text;
    }

    private void updateSlider(Cs_slider cs_slider, int value, int maxValue)
    {
        cs_slider.slider.value = value;
        cs_slider.text.text = cs_slider.text.text.Split('=')[0] + ("= " + value + "/" + maxValue);
    }

    public void Update()
    {
        if (currentPlayer != null)
        {
            if (currentPlayer.inventory != null)
            {
                updatePlayer();
            }
            
        }
    }

    public void setPlayer(Player player)
    {
        currentPlayer = player;
        maxStats = new int[stats.Length];
        maxStats[0] = currentPlayer.health;
        maxStats[1] = 100;
        maxStats[2] = 100;

        maxStats[3] = currentPlayer.stamina;
        maxStats[4] = 100;
        maxStats[5] = currentPlayer.mana;
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i].slider.maxValue = maxStats[i];
        }
    }

}
