using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GearArgs
{
    public int Level;
    public int Value;
    public float Health;
    public float Mana;
    public float Damage;
    public float Speed;
    public float CooldownReduction;
    public float Armor;
    public float Evasion;
    public float Leech;
    public float Area;

    public float InnateValue;
    public float FamilyValue;
    public float SecondaryValue;
    public float TertiaryValue;
    public float QuaternaryValue;

    public Gear.GearType ThisGearType;
    public Gear.Rarity ThisRarity;
    public Gear.StatType InnateStatType;
    public Gear.StatType FamilyStatType;
    public Gear.StatType SecondaryStatType;
    public Gear.StatType TertiaryStatType;
    public Gear.StatType QuaternaryStatType;

    public string Suffix;
    public Sprite Sprite;

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
}
