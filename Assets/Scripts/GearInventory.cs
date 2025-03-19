using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearInventory : MonoBehaviour
{
    public event EventHandler<Gear> OnGearEquipped;
    public event EventHandler<Gear> OnGearUnequipped;

    public List<Gear> GearList { get; private set; } = new List<Gear>();
    public List<Gear> EquippedGearList { get; private set; } = new List<Gear>();

    private Gear _mainHand;
    private Gear _offHand;
    private Gear _helmet;
    private Gear _bodyArmor;
    private Gear _ring;

    private const int GEAR_LIST_COUNT_MAX = 12;

    private void Start()
    {
        GearGenerator.OnGearGenerated += GearGenerator_OnGearGenerated;
    }

    private void GearGenerator_OnGearGenerated(object sender, Gear gear)
    {
        AddToList(GearList, gear);

        if (GearList.Count > GEAR_LIST_COUNT_MAX)
        {
            SellGear(GearList[0]);
        }
    }

    public void SafeEquipGear(Gear gear)
    {
        if (IsGearSlotEmpty(gear.ThisGearType))
        {
            EquipGear(gear);
        }
        else
        {
            UnequipGear(GetEquippedGearByType(gear.ThisGearType), true);
            EquipGear(gear);
        }
    }

    private void EquipGear(Gear gear)
    {
        switch (gear.ThisGearType)
        {
            case Gear.GearType.MainHand:
                _mainHand = gear;
                RemoveFromList(GearList, gear);
                AddToList(EquippedGearList, gear);
                break;
            case Gear.GearType.OffHand:
                _offHand = gear;
                RemoveFromList(GearList, gear);
                AddToList(EquippedGearList, gear);
                break;
            case Gear.GearType.Helmet: 
                _helmet = gear;
                RemoveFromList(GearList, gear);
                AddToList(EquippedGearList, gear);
                break;
                case Gear.GearType.BodyArmor: 
                _bodyArmor = gear;
                RemoveFromList(GearList, gear);
                AddToList(EquippedGearList, gear);
                break;
            case Gear.GearType.Ring: 
                _ring = gear;
                RemoveFromList(GearList, gear);
                AddToList(EquippedGearList, gear);
                break;
        }

        OnGearEquipped?.Invoke(this, gear);
    }

    public void UnequipGear(Gear gear, bool sellGearValue)
    {
        switch (gear.ThisGearType)
        {
            case Gear.GearType.MainHand:
                RemoveFromList(EquippedGearList, _mainHand);
                _mainHand = null;
                break;
            case Gear.GearType.OffHand:
                RemoveFromList(EquippedGearList, _offHand);
                _offHand = null;
                break;
            case Gear.GearType.Helmet:
                RemoveFromList(EquippedGearList, _helmet);
                _helmet = null;
                break;
            case Gear.GearType.BodyArmor:
                RemoveFromList(EquippedGearList, _bodyArmor);
                _bodyArmor = null;
                break;
            case Gear.GearType.Ring:
                RemoveFromList(EquippedGearList, _ring);
                _ring = null;
                break;
        }
        OnGearUnequipped?.Invoke(this, gear);
        if (sellGearValue)
        {
            SellGear(gear);
        }
    }

    public bool IsGearSlotEmpty(Gear.GearType gearType)
    {
        switch (gearType)
        {
            case Gear.GearType.MainHand:
                return _mainHand == null ? true : false;
            case Gear.GearType.OffHand:
                return _offHand == null ? true : false;
            case Gear.GearType.Helmet:
                return _helmet == null ? true : false;
            case Gear.GearType.BodyArmor:
                return _bodyArmor == null ? true : false;
            case Gear.GearType.Ring:
                return _ring == null ? true : false;
        }
        return false;
    }

    public Gear GetEquippedGearByType(Gear.GearType gearType)
    {
        switch (gearType)
        {
            case Gear.GearType.MainHand:
                return _mainHand;
            case Gear.GearType.OffHand:
                return _offHand;
            case Gear.GearType.Helmet:
                return _helmet;
            case Gear.GearType.BodyArmor:
                return _bodyArmor;
            default:
                return _ring;
        }
    }

    private void AddToList(List<Gear> list, Gear gear)
    {
        list.Add(gear);
    }

    private void RemoveFromList(List<Gear> list, Gear gear)
    {
        list.Remove(gear);
    }

    private void SellGear(Gear gear)
    {
        if (TryGetComponent(out MetaInventory metaInventory))
        {
            metaInventory.AddGold(gear.Value);
        }
        RemoveFromList(GearList, gear);

        Destroy(gear.gameObject);
    }

    public void SellAllGear()
    {
        List<Gear> tempGearList = new List<Gear>();
        List<Gear> tempEquippedGearList = new List<Gear>();

        //Get the gold
        foreach (Gear gear in GearList)
        {
            if (TryGetComponent(out MetaInventory metaInventory))
            {
                metaInventory.AddGold(gear.Value);
            }
            tempGearList.Add(gear);
        }
        //Get the gold
        //Unequip the gear
        foreach (Gear gear in EquippedGearList)
        {
            if (TryGetComponent(out MetaInventory metaInventory))
            {
                metaInventory.AddGold(gear.Value);
            }
            tempEquippedGearList.Add(gear);
        }

        //Remove from lists
        foreach (Gear gear in tempGearList)
        {
            RemoveFromList(GearList, gear);

            Destroy(gear.gameObject);
        }
        foreach (Gear gear in tempEquippedGearList)
        {
            UnequipGear(gear, false);
            Destroy(gear.gameObject);
        }
    }

    private void OnDestroy()
    {
        GearGenerator.OnGearGenerated -= GearGenerator_OnGearGenerated;
    }
}
