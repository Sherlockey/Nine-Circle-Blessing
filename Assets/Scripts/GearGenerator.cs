using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearGenerator : MonoBehaviour
{
    public static event EventHandler<Gear> OnGearGenerated;

    [SerializeField] Gear _gearPrefab;
    [SerializeField] Sprite[] _sprites;

    public static GearGenerator Instance;

    private const float HEALTH_MAX_PER_ITEM = 180f;
    private const float MANA_MAX_PER_ITEM = 52.64f;
    private const float DAMAGE_MAX_PER_ITEM = 62.64f;
    private const float SPEED_MAX_PER_ITEM = 18.04f;
    private const float COOLDOWN_REDUCTION_MAX_PER_ITEM = 18.04f;
    private const float ARMOR_MAX_PER_ITEM = 18.04f;
    private const float EVASION_MAX_PER_ITEM = 16.04f;
    private const float LEECH_MAX_PER_ITEM = 9.02f;
    private const float AREA_MAX_PER_ITEM = 9.02f;

    private const float INNATE_SPLIT_SCALAR = 0.7f;
    private const float FAMILY_SPLIT_SCALAR = 0.3f;
    private const float DAMAGE_INNATE_SCALAR = 2.5f;
    private const float HEALTH_INNATE_SCALAR = 5/3f;
    private const float FAMILY_WHITE_SCALAR = 1f;
    private const float FAMILY_BLUE_SCALAR = 0.9f;
    private const float FAMILY_YELLOW_SCALAR = 0.8f;
    private const float FAMILY_ORANGE_SCALAR = 0.7f;
    private const float MAIN_HAND_SCALAR = 1.3f;
    private const float OFF_HAND_SCALAR = 0.7f;
    private const float NONWEAPON_SCALAR = 1f;
    private const float ROLL_SCALAR_MIN = 0.7f;
    private const float ROLL_SCALAR_MAX = 1f;

    private const int VALUE_SCALAR = 10;

    private const string HEALTH_SUFFIX = "of Gluttony";
    private const string MANA_SUFFIX = "of Limbo";
    private const string DAMAGE_SUFFIX = "of Violence";
    private const string SPEED_SUFFIX = "of Wrath";
    private const string COOLDOWN_REDUCTION_SUFFIX = "of Fraud";
    private const string ARMOR_SUFFIX = "of Greed";
    private const string EVASION_SUFFIX = "of Heresy";
    private const string LEECH_SUFFIX = "of Treachery";
    private const string AREA_SUFFIX = "of Lust";

    private float _chanceToDropGear = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (GameObject enemy in BattleManager.Instance.EnemyList)
        {
            Stats enemyStats = enemy.GetComponent<Stats>();
            enemyStats.OnDead += EnemyStats_OnDead;
        }
    }

    private void EnemyStats_OnDead(object sender, GameObject enemy)
    {
        if (UnityEngine.Random.value <= _chanceToDropGear)
        {
            GenerateGear();
        }
    }

    public void GenerateGear()
    {
        GearArgs gearArgs = new GearArgs();

        //Set the level, type, rarity, innate stat type, family stat type, and low roll scalar
        gearArgs.Level = GameManager.Instance.CurrentCircleNumber;
        gearArgs.Value = gearArgs.Level * VALUE_SCALAR;
        gearArgs.ThisGearType = (Gear.GearType)UnityEngine.Random.Range(0,Enum.GetNames(typeof(Gear.GearType)).Length);
        gearArgs.ThisRarity = (Gear.Rarity)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Gear.Rarity)).Length);
        gearArgs.InnateStatType = LookupInnateStatType(gearArgs);
        gearArgs.FamilyStatType = (Gear.StatType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Gear.StatType)).Length);
        float rollScalar = UnityEngine.Random.Range(ROLL_SCALAR_MIN, ROLL_SCALAR_MAX);

        //Determine the necessary additional stat types
        int additionalTypeCount = 0;

        switch (gearArgs.ThisRarity)
        {
            case Gear.Rarity.White:
                additionalTypeCount = 0;
                break;
            case Gear.Rarity.Blue:
                additionalTypeCount = 1;
                break;
            case Gear.Rarity.Yellow:
                additionalTypeCount = 2;
                break;
            case Gear.Rarity.Orange:
                additionalTypeCount = 3;
                break;
        }

        int i = additionalTypeCount;

        if (i > 0)
        {
            //Make a list containing all possible StatTypes
            List<Gear.StatType> possibleStatTypes = new List<Gear.StatType>();
            foreach (int j in Enum.GetValues(typeof(Gear.StatType)))
            {
                possibleStatTypes.Add((Gear.StatType)j);
            }

            //Remove the Family StatType from the list
            possibleStatTypes.Remove(gearArgs.FamilyStatType);

            //Create an int for the random index we will pick
            int randomIndex = UnityEngine.Random.Range(0, possibleStatTypes.Count);

            //Assign Secondary StatType
            gearArgs.SecondaryStatType = possibleStatTypes[randomIndex];

            //Remove Secondary StatType from the list
            possibleStatTypes.Remove(gearArgs.SecondaryStatType);
            i--;

            //Continue until there are no more additional types remaining
            if (i > 0)
            {
                randomIndex = UnityEngine.Random.Range(0, possibleStatTypes.Count);
                gearArgs.TertiaryStatType = possibleStatTypes[randomIndex];

                possibleStatTypes.Remove(gearArgs.TertiaryStatType);
                i--;
            }
            if (i > 0)
            {
                randomIndex = UnityEngine.Random.Range(0, possibleStatTypes.Count);
                gearArgs.QuaternaryStatType = possibleStatTypes[randomIndex];
            }
        }

        //Calculate Innate Value
        switch (gearArgs.ThisGearType)
        {
            case Gear.GearType.MainHand:
            case Gear.GearType.OffHand:
                gearArgs.Damage += LookupInnateCalculations(gearArgs, rollScalar);
                gearArgs.InnateValue = LookupInnateCalculations(gearArgs, rollScalar);
                break;
            default:
                gearArgs.Health += LookupInnateCalculations(gearArgs, rollScalar);
                gearArgs.InnateValue = LookupInnateCalculations(gearArgs, rollScalar);
                break;
        }

        //Calculate Family Value
        switch (gearArgs.FamilyStatType)
        {
            case Gear.StatType.Health:
                gearArgs.Health += LookupFamilyCalculations(gearArgs, rollScalar);
                gearArgs.FamilyValue = LookupFamilyCalculations(gearArgs, rollScalar);
                break;
            case Gear.StatType.Mana:
                gearArgs.Mana += LookupFamilyCalculations(gearArgs, rollScalar);
                gearArgs.FamilyValue = LookupFamilyCalculations(gearArgs, rollScalar);
                break;
            case Gear.StatType.Damage:
                gearArgs.Damage += LookupFamilyCalculations(gearArgs, rollScalar);
                gearArgs.FamilyValue = LookupFamilyCalculations(gearArgs, rollScalar);
                break;
            case Gear.StatType.Speed:
                gearArgs.Speed += LookupFamilyCalculations(gearArgs, rollScalar);
                gearArgs.FamilyValue = LookupFamilyCalculations(gearArgs, rollScalar);
                break;
            case Gear.StatType.CooldownReduction:
                gearArgs.CooldownReduction += LookupFamilyCalculations(gearArgs, rollScalar);
                gearArgs.FamilyValue = LookupFamilyCalculations(gearArgs, rollScalar);
                break;
            case Gear.StatType.Armor:
                gearArgs.Armor += LookupFamilyCalculations(gearArgs, rollScalar);
                gearArgs.FamilyValue = LookupFamilyCalculations(gearArgs, rollScalar);
                break;
            case Gear.StatType.Evasion:
                gearArgs.Evasion += LookupFamilyCalculations(gearArgs, rollScalar);
                gearArgs.FamilyValue = LookupFamilyCalculations(gearArgs, rollScalar);
                break;
            case Gear.StatType.Leech:
                gearArgs.Leech += LookupFamilyCalculations(gearArgs, rollScalar);
                gearArgs.FamilyValue = LookupFamilyCalculations(gearArgs, rollScalar);
                break;
            case Gear.StatType.Area:
                gearArgs.Area += LookupFamilyCalculations(gearArgs, rollScalar);
                gearArgs.FamilyValue = LookupFamilyCalculations(gearArgs, rollScalar);
                break;
        }

        int k = additionalTypeCount;

        //Calculate Additional Values
        if (k > 0)
        {
            switch (gearArgs.SecondaryStatType)
            {
                case Gear.StatType.Health:
                    gearArgs.Health += LookupSecondaryCalculations(gearArgs, rollScalar);
                    gearArgs.SecondaryValue = LookupSecondaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Mana:
                    gearArgs.Mana += LookupSecondaryCalculations(gearArgs, rollScalar);
                    gearArgs.SecondaryValue = LookupSecondaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Damage:
                    gearArgs.Damage += LookupSecondaryCalculations(gearArgs, rollScalar);
                    gearArgs.SecondaryValue = LookupSecondaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Speed:
                    gearArgs.Speed += LookupSecondaryCalculations(gearArgs, rollScalar);
                    gearArgs.SecondaryValue = LookupSecondaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.CooldownReduction:
                    gearArgs.CooldownReduction += LookupSecondaryCalculations(gearArgs, rollScalar);
                    gearArgs.SecondaryValue = LookupSecondaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Armor:
                    gearArgs.Armor += LookupSecondaryCalculations(gearArgs, rollScalar);
                    gearArgs.SecondaryValue = LookupSecondaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Evasion:
                    gearArgs.Evasion += LookupSecondaryCalculations(gearArgs, rollScalar);
                    gearArgs.SecondaryValue = LookupSecondaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Leech:
                    gearArgs.Leech += LookupSecondaryCalculations(gearArgs, rollScalar);
                    gearArgs.SecondaryValue = LookupSecondaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Area:
                    gearArgs.Area += LookupSecondaryCalculations(gearArgs, rollScalar);
                    gearArgs.SecondaryValue = LookupSecondaryCalculations(gearArgs, rollScalar);
                    break;
            }

            k--;
        }
        if (k > 0)
        {
            switch (gearArgs.TertiaryStatType)
            {
                case Gear.StatType.Health:
                    gearArgs.Health += LookupTertiaryCalculations(gearArgs, rollScalar);
                    gearArgs.TertiaryValue = LookupTertiaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Mana:
                    gearArgs.Mana += LookupTertiaryCalculations(gearArgs, rollScalar);
                    gearArgs.TertiaryValue = LookupTertiaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Damage:
                    gearArgs.Damage += LookupTertiaryCalculations(gearArgs, rollScalar);
                    gearArgs.TertiaryValue = LookupTertiaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Speed:
                    gearArgs.Speed += LookupTertiaryCalculations(gearArgs, rollScalar);
                    gearArgs.TertiaryValue = LookupTertiaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.CooldownReduction:
                    gearArgs.CooldownReduction += LookupTertiaryCalculations(gearArgs, rollScalar);
                    gearArgs.TertiaryValue = LookupTertiaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Armor:
                    gearArgs.Armor += LookupTertiaryCalculations(gearArgs, rollScalar);
                    gearArgs.TertiaryValue = LookupTertiaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Evasion:
                    gearArgs.Evasion += LookupTertiaryCalculations(gearArgs, rollScalar);
                    gearArgs.TertiaryValue = LookupTertiaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Leech:
                    gearArgs.Leech += LookupTertiaryCalculations(gearArgs, rollScalar);
                    gearArgs.TertiaryValue = LookupTertiaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Area:
                    gearArgs.Area += LookupTertiaryCalculations(gearArgs, rollScalar);
                    gearArgs.TertiaryValue = LookupTertiaryCalculations(gearArgs, rollScalar);
                    break;
            }

            k--;
        }
        if (k > 0)
        {
            switch (gearArgs.QuaternaryStatType)
            {
                case Gear.StatType.Health:
                    gearArgs.Health += LookupQuaternaryCalculations(gearArgs, rollScalar);
                    gearArgs.QuaternaryValue = LookupQuaternaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Mana:
                    gearArgs.Mana += LookupQuaternaryCalculations(gearArgs, rollScalar);
                    gearArgs.QuaternaryValue = LookupQuaternaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Damage:
                    gearArgs.Damage += LookupQuaternaryCalculations(gearArgs, rollScalar);
                    gearArgs.QuaternaryValue = LookupQuaternaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Speed:
                    gearArgs.Speed += LookupQuaternaryCalculations(gearArgs, rollScalar);
                    gearArgs.QuaternaryValue = LookupQuaternaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.CooldownReduction:
                    gearArgs.CooldownReduction += LookupQuaternaryCalculations(gearArgs, rollScalar);
                    gearArgs.QuaternaryValue = LookupQuaternaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Armor:
                    gearArgs.Armor += LookupQuaternaryCalculations(gearArgs, rollScalar);
                    gearArgs.QuaternaryValue = LookupQuaternaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Evasion:
                    gearArgs.Evasion += LookupQuaternaryCalculations(gearArgs, rollScalar);
                    gearArgs.QuaternaryValue = LookupQuaternaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Leech:
                    gearArgs.Leech += LookupQuaternaryCalculations(gearArgs, rollScalar);
                    gearArgs.QuaternaryValue = LookupQuaternaryCalculations(gearArgs, rollScalar);
                    break;
                case Gear.StatType.Area:
                    gearArgs.Area += LookupQuaternaryCalculations(gearArgs, rollScalar);
                    gearArgs.QuaternaryValue = LookupQuaternaryCalculations(gearArgs, rollScalar);
                    break;
            }
        }

        //Set the suffix and the image of the gear
        gearArgs.Suffix = LookupSuffix(gearArgs);
        gearArgs.Sprite = SetSprite(gearArgs);

        //Create the new piece of gear and set its values to the GearArgs struct created above
        Gear newGear = Instantiate(_gearPrefab);
        newGear.Init(gearArgs);
        OnGearGenerated?.Invoke(this, newGear);
    }

    private float LookupStatTypeMax(Gear.StatType statType)
    {
        switch (statType)
        {
            case Gear.StatType.Health:
                return HEALTH_MAX_PER_ITEM;
            case Gear.StatType.Mana:
                return MANA_MAX_PER_ITEM;
            case Gear.StatType.Damage:
                return DAMAGE_MAX_PER_ITEM;
            case Gear.StatType.Speed:
                return SPEED_MAX_PER_ITEM;
            case Gear.StatType.CooldownReduction:
                return COOLDOWN_REDUCTION_MAX_PER_ITEM;
            case Gear.StatType.Armor:
                return ARMOR_MAX_PER_ITEM;
            case Gear.StatType.Evasion:
                return EVASION_MAX_PER_ITEM;
            case Gear.StatType.Leech:
                return LEECH_MAX_PER_ITEM;
            case Gear.StatType.Area:
                return AREA_MAX_PER_ITEM;
        }
        //is this how i should handle this? or should i just default to something? idk
        return -1f;
    }

    private float LookupLevelScalar()
    {
        float levelScalar = (float)GameManager.Instance.CurrentCircleNumber / (float)GameManager.Instance.MAX_CIRCLE_NUMBER;
        return levelScalar;
    }

    private float LookupInnateTypeScalar(Gear.StatType statType)
    {
        switch (statType)
        {
            case Gear.StatType.Damage:
                return DAMAGE_INNATE_SCALAR;
            default:
                return HEALTH_INNATE_SCALAR;
        }
    }

    private float LookupGearTypeScalar(Gear.GearType gearType)
    {
        switch (gearType)
        {
            case Gear.GearType.MainHand:
                return MAIN_HAND_SCALAR;
            case Gear.GearType.OffHand:
                return OFF_HAND_SCALAR;
            default:
                return NONWEAPON_SCALAR;
        }
    }

    private float LookupFamilyRarityScalar(Gear.Rarity rarity)
    {
        switch (rarity)
        {
            case Gear.Rarity.White:
                return FAMILY_WHITE_SCALAR;
            case Gear.Rarity.Blue:
                return FAMILY_BLUE_SCALAR;
            case Gear.Rarity.Yellow:
                return FAMILY_YELLOW_SCALAR;
            case Gear.Rarity.Orange:
                return FAMILY_ORANGE_SCALAR;
        }
        //is this how i should handle this? or should i just default to something? idk
        return -1f;
    }

    private float LookupInnateCalculations(GearArgs gearArgs, float rollScalar)
    {
        float value = Mathf.Ceil(LookupStatTypeMax(gearArgs.InnateStatType) * LookupLevelScalar() * INNATE_SPLIT_SCALAR * LookupInnateTypeScalar(gearArgs.InnateStatType)
            * LookupGearTypeScalar(gearArgs.ThisGearType) * rollScalar);

        return value;
    }

    private float LookupFamilyCalculations(GearArgs gearArgs, float rollScalar)
    {
        float value = Mathf.Ceil(LookupStatTypeMax(gearArgs.FamilyStatType) * LookupLevelScalar() * FAMILY_SPLIT_SCALAR * LookupFamilyRarityScalar(gearArgs.ThisRarity)
            * LookupGearTypeScalar(gearArgs.ThisGearType) * rollScalar);

        return value;
    }

    private float LookupSecondaryCalculations(GearArgs gearArgs, float rollScalar)
    {
        float value = Mathf.Ceil(LookupStatTypeMax(gearArgs.SecondaryStatType) * LookupLevelScalar() * FAMILY_SPLIT_SCALAR * LookupFamilyRarityScalar(gearArgs.ThisRarity)
            * LookupGearTypeScalar(gearArgs.ThisGearType) * rollScalar);

        return value;
    }

    private float LookupTertiaryCalculations(GearArgs gearArgs, float rollScalar)
    {
        float value = Mathf.Ceil(LookupStatTypeMax(gearArgs.TertiaryStatType) * LookupLevelScalar() * FAMILY_SPLIT_SCALAR * LookupFamilyRarityScalar(gearArgs.ThisRarity)
            * LookupGearTypeScalar(gearArgs.ThisGearType) * rollScalar);

        return value;
    }

    private float LookupQuaternaryCalculations(GearArgs gearArgs, float rollScalar)
    {
        float value = Mathf.Ceil(LookupStatTypeMax(gearArgs.QuaternaryStatType) * LookupLevelScalar() * FAMILY_SPLIT_SCALAR * LookupFamilyRarityScalar(gearArgs.ThisRarity)
            * LookupGearTypeScalar(gearArgs.ThisGearType) * rollScalar);

        return value;
    }

    private Gear.StatType LookupInnateStatType(GearArgs gearArgs)
    {
        switch (gearArgs.ThisGearType)
        {
            case Gear.GearType.MainHand:
            case Gear.GearType.OffHand:
                return Gear.StatType.Damage;
            default:
                return Gear.StatType.Health;
        }
    }

    private string LookupSuffix(GearArgs gearArgs)
    {
        switch (gearArgs.FamilyStatType)
        {
            case Gear.StatType.Health:
                return HEALTH_SUFFIX;
            case Gear.StatType.Mana:
                return MANA_SUFFIX;
            case Gear.StatType.Damage:
                return DAMAGE_SUFFIX;
            case Gear.StatType.Speed:
                return SPEED_SUFFIX;
            case Gear.StatType.CooldownReduction:
                return COOLDOWN_REDUCTION_SUFFIX;
            case Gear.StatType.Armor:
                return ARMOR_SUFFIX;
            case Gear.StatType.Evasion:
                return EVASION_SUFFIX;
            case Gear.StatType.Leech:
                return LEECH_SUFFIX;
            case Gear.StatType.Area:
                return AREA_SUFFIX;
        }
        return null;
    }

    private Sprite SetSprite(GearArgs gearArgs)
    {
        switch (gearArgs.ThisGearType)
        {
            case Gear.GearType.MainHand:
                return _sprites[0];
            case Gear.GearType.OffHand:
                return _sprites[1];
            case Gear.GearType.Helmet:
                return _sprites[2];
            case Gear.GearType.BodyArmor:
                return _sprites[3];
            case Gear.GearType.Ring:
                return _sprites[4];
        }
        return null;
    }
}
