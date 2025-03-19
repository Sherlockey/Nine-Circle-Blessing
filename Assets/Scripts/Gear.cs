using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gear : MonoBehaviour
{
    public int Level {  get; private set; }
    public int Value { get; private set; }
    public float Health { get; private set; }
    public float Mana { get; private set; }
    public float Damage { get; private set; }
    public float Speed { get; private set; }
    public float CooldownReduction { get; private set; }
    public float Armor { get; private set; }
    public float Evasion { get; private set; }
    public float Leech { get; private set; }
    public float Area { get; private set; }

    public float InnateValue { get; private set; }
    public float FamilyValue { get; private set; }
    public float SecondaryValue { get; private set; }
    public float TertiaryValue { get; private set; }
    public float QuaternaryValue { get; private set; }


    public GearType ThisGearType { get; private set; }
    public Rarity ThisRarity { get; private set; }
    public StatType InnateStatType { get; private set; }
    public StatType FamilyStatType { get; private set; }
    public StatType SecondaryStatType { get; private set; }
    public StatType TertiaryStatType { get; private set; }
    public StatType QuaternaryStatType { get; private set; }

    public string Suffix { get; private set; }
    public Sprite Sprite { get; private set; }

    public enum GearType
    {
        MainHand,
        OffHand,
        Helmet,
        BodyArmor,
        Ring,
    }

    public enum Rarity
    {
        White,
        Blue,
        Yellow,
        Orange,
    }

    public enum StatType
    {
        Health,
        Mana,
        Damage,
        Speed,
        CooldownReduction,
        Armor,
        Evasion,
        Leech,
        Area,
    }

    public void Init(GearArgs gearArgs)
    {
        Level = gearArgs.Level;
        Value = gearArgs.Value;
        Health = gearArgs.Health;
        Mana = gearArgs.Mana;
        Damage = gearArgs.Damage;
        Speed = gearArgs.Speed;
        CooldownReduction  = gearArgs.CooldownReduction;
        Armor = gearArgs.Armor;
        Evasion = gearArgs.Evasion;
        Leech = gearArgs.Leech;
        Area = gearArgs.Area;
        ThisGearType = gearArgs.ThisGearType;
        ThisRarity = gearArgs.ThisRarity;
        InnateStatType = gearArgs.InnateStatType;
        FamilyStatType = gearArgs.FamilyStatType;
        SecondaryStatType = gearArgs.SecondaryStatType;
        TertiaryStatType = gearArgs.TertiaryStatType;
        QuaternaryStatType = gearArgs.QuaternaryStatType;
        Suffix = gearArgs.Suffix;
        Sprite = gearArgs.Sprite;

        InnateValue = gearArgs.InnateValue;
        FamilyValue = gearArgs.FamilyValue;
        SecondaryValue = gearArgs.SecondaryValue;
        TertiaryValue = gearArgs.TertiaryValue;
        QuaternaryValue = gearArgs.QuaternaryValue;

        switch (ThisRarity)
        {
            case Rarity.White:
                transform.GetChild(0).GetComponent<Image>().color = new Color(0.65f, 0.65f, 0.65f);
                break;
            case Rarity.Blue:
                transform.GetChild(0).GetComponent<Image>().color = new Color(0.28f, 0.24f, 0.71f);
                break;
            case Rarity.Yellow:
                transform.GetChild(0).GetComponent<Image>().color = new Color(0.77f, 0.72f, 0.12f);
                break;
            case Rarity.Orange:
                transform.GetChild(0).GetComponent<Image>().color = new Color(0.61f, 0.36f, 0.17f);
                break;
        }

        transform.GetChild(1).GetComponent<Image>().sprite = Sprite;
        transform.GetChild(2).GetComponent<TMP_Text>().text = Level.ToString();
    }
}
