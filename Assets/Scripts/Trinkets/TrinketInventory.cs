using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class TrinketInventory : MonoBehaviour
{
    [SerializeField] private Trinket[] _trinketArray;
    private Trinket _equippedTrinket;

    public void SetTrinketEnabled(Trinket trinket, bool value)
    {
        if (trinket.GetEnabled() != value)
        {
            trinket.SetEnabled(value);
            if (value == true)
            {
                _equippedTrinket = trinket;
            }
            else if (value == false)
            {
                _equippedTrinket = null;
            }
        }
    }

    public void SetTrinketOwned(Trinket trinket, bool value)
    {
        if (trinket.GetOwned() != value)
        {
            trinket.SetOwned(value);
        }
    }

    public Trinket GetTrinketFromTrinketArrayByString(string s)
    {
        for (int i = 0; i < _trinketArray.Length; i++)
        {
            if (s == _trinketArray[i].GetSuffix())
            {
                return _trinketArray[i];
            }
        }
        return null;
    }

    public Trinket[] GetTrinketArray()
    {
        return _trinketArray;
    }

    public Trinket GetEquippedTrinket()
    {
        return _equippedTrinket;
    }
}
