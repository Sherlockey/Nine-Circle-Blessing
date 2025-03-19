using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GearTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _gearTooltipPrefab;
    [SerializeField] private GameObject _gearTooltipNamePrefab;
    [SerializeField] private GameObject _gearTooltipValuePrefab;

    private GameObject _gearTooltip;
    private GameObject _comparisonGearTooltip;
    private GameObject _canvas;

    private const int PADDING_SIZE = 20;
    private const int ITEM_NAME_SIZE = 50;
    private const int CELL_SIZE = 50;
    private const int BACKGROUND_WIDTH = 500;

    private static readonly Vector2 INVENTORY_POSITION = new Vector2(300, 300);
    private static readonly Vector2 EQUIPMENT_POSITION = new Vector2(300, -40);

    private void Start()
    {
        _canvas = GameObject.Find("BattleCanvas");
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (_gearTooltip == null)
        {
            Gear gear = data.pointerEnter.GetComponent<Gear>();

            if (gear != BattleManager.Instance.Player.GetComponent<GearInventory>().GetEquippedGearByType(gear.ThisGearType))
            {
                _gearTooltip = GenerateTooltip(gear, INVENTORY_POSITION);
            }
            else
            {
                _comparisonGearTooltip = GenerateTooltip(gear, EQUIPMENT_POSITION);
            }


            //if gear.GearType check if already equipped something of that type then generate comparision gear tooltip etc
            if (!BattleManager.Instance.Player.GetComponent<GearInventory>().IsGearSlotEmpty(gear.ThisGearType))
            {
                if (gear != BattleManager.Instance.Player.GetComponent<GearInventory>().GetEquippedGearByType(gear.ThisGearType))
                {
                    _comparisonGearTooltip = GenerateTooltip(BattleManager.Instance.Player.GetComponent<GearInventory>().GetEquippedGearByType(gear.ThisGearType), EQUIPMENT_POSITION);
                }
            }
        }
    }

    private GameObject GenerateTooltip(Gear gear, Vector2 position)
    {
        GameObject tooltip = Instantiate(_gearTooltipPrefab, _canvas.transform);
        tooltip.GetComponent<RectTransform>().localPosition = position;
        //+2 is shifting the ThisRarity int casted enum over the appropriate amount
        int numberOfStats = (int)gear.ThisRarity + 2;
        tooltip.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(BACKGROUND_WIDTH, PADDING_SIZE + ITEM_NAME_SIZE + CELL_SIZE * numberOfStats);

        //set the name of the item
        GameObject gearTooltipGearName = Instantiate(_gearTooltipNamePrefab, tooltip.transform.GetChild(0).GetChild(0));
        gearTooltipGearName.GetComponent<TMP_Text>().text = GetStringFromGearType(gear.ThisGearType) + " " + gear.Suffix;
        gearTooltipGearName.GetComponent<TMP_Text>().fontStyle |= FontStyles.Underline;
        GameObject gearTooltipValueBlank = Instantiate(_gearTooltipValuePrefab, tooltip.transform.GetChild(0).GetChild(1));
        gearTooltipValueBlank.GetComponent<TMP_Text>().text = "";

        //setting first gearTooltipName to gear.InnateStatType
        GameObject gearTooltipNameInnate = Instantiate(_gearTooltipNamePrefab, tooltip.transform.GetChild(0).GetChild(0));
        gearTooltipNameInnate.GetComponent<TMP_Text>().text = GetStringFromStatType(gear.InnateStatType);

        //setting first gearTooltipValue to gear.InnateStatValue
        GameObject gearTooltipValueInnate = Instantiate(_gearTooltipValuePrefab, tooltip.transform.GetChild(0).GetChild(1));
        gearTooltipValueInnate.GetComponent<TMP_Text>().text = gear.InnateValue.ToString();

        numberOfStats--;

        //setting second gearTooltipName to gear.FamilyStatType
        GameObject gearTooltipNameFamily = Instantiate(_gearTooltipNamePrefab, tooltip.transform.GetChild(0).GetChild(0));
        gearTooltipNameFamily.GetComponent<TMP_Text>().text = GetStringFromStatType(gear.FamilyStatType);

        //setting second gearTooltipValue to gear.FamilyStatValue
        GameObject gearTooltipValueFamily = Instantiate(_gearTooltipValuePrefab, tooltip.transform.GetChild(0).GetChild(1));
        gearTooltipValueFamily.GetComponent<TMP_Text>().text = gear.FamilyValue.ToString();

        numberOfStats--;

        if (numberOfStats > 0)
        {
            //setting third gearTooltipName to gear.FamilyStatType
            GameObject gearTooltipNameSecondary = Instantiate(_gearTooltipNamePrefab, tooltip.transform.GetChild(0).GetChild(0));
            gearTooltipNameSecondary.GetComponent<TMP_Text>().text = GetStringFromStatType(gear.SecondaryStatType);

            //setting third gearTooltipValue to gear.FamilyStatValue
            GameObject gearTooltipValueSecondary = Instantiate(_gearTooltipValuePrefab, tooltip.transform.GetChild(0).GetChild(1));
            gearTooltipValueSecondary.GetComponent<TMP_Text>().text = gear.SecondaryValue.ToString();

            numberOfStats--;
        }

        if (numberOfStats > 0)
        {
            //setting fourth gearTooltipName to gear.FamilyStatType
            GameObject gearTooltipNameTertiary = Instantiate(_gearTooltipNamePrefab, tooltip.transform.GetChild(0).GetChild(0));
            gearTooltipNameTertiary.GetComponent<TMP_Text>().text = GetStringFromStatType(gear.TertiaryStatType);

            //setting fourth gearTooltipValue to gear.FamilyStatValue
            GameObject gearTooltipValueTertiary = Instantiate(_gearTooltipValuePrefab, tooltip.transform.GetChild(0).GetChild(1));
            gearTooltipValueTertiary.GetComponent<TMP_Text>().text = gear.TertiaryValue.ToString();

            numberOfStats--;
        }

        if (numberOfStats > 0)
        {
            //setting fifth gearTooltipName to gear.FamilyStatType
            GameObject gearTooltipNameQuaternary = Instantiate(_gearTooltipNamePrefab, tooltip.transform.GetChild(0).GetChild(0));
            gearTooltipNameQuaternary.GetComponent<TMP_Text>().text = GetStringFromStatType(gear.QuaternaryStatType);

            //setting fifth gearTooltipValue to gear.FamilyStatValue
            GameObject gearTooltipValueQuaternary = Instantiate(_gearTooltipValuePrefab, tooltip.transform.GetChild(0).GetChild(1));
            gearTooltipValueQuaternary.GetComponent<TMP_Text>().text = gear.QuaternaryValue.ToString();
        }

        return tooltip;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (_gearTooltip != null)
        {
            Destroy(_gearTooltip);
        }
        if (_comparisonGearTooltip != null)
        {
            Destroy(_comparisonGearTooltip);
        }
    }

    private void OnDestroy()
    {
        if (_gearTooltip != null)
        {
            Destroy(_gearTooltip);
        }
        if (_comparisonGearTooltip != null)
        {
            Destroy(_comparisonGearTooltip);
        }
    }

    private string GetStringFromStatType(Gear.StatType statType)
    {
        switch (statType)
        {
            case Gear.StatType.Health:
                return "Health";
            case Gear.StatType.Mana:
                return "Mana";
            case Gear.StatType.Damage:
                return "Damage";
            case Gear.StatType.Speed:
                return "Speed";
            case Gear.StatType.CooldownReduction:
                return "Cooldown Reduction";
            case Gear.StatType.Armor:
                return "Armor";
            case Gear.StatType.Evasion:
                return "Evasion";
            case Gear.StatType.Area:
                return "Area";
            case Gear.StatType.Leech:
                return "Leech";
        }
        return null;
    }

    private string GetStringFromGearType(Gear.GearType gearType)
    {
        switch (gearType)
        {
            case Gear.GearType.MainHand:
                return "Staff";
            case Gear.GearType.OffHand:
                return "Wand";
            case Gear.GearType.BodyArmor:
                return "Cloak";
            case Gear.GearType.Helmet:
                return "Circlet";
            case Gear.GearType.Ring:
                return "Ring";
        }
        return null;
    }
}
