using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaInventory : MonoBehaviour
{
    public event EventHandler<int> OnGoldChanged;

    private int _gold = 0;

    //TODO field here for trinket

    public void AddGold(int amount)
    {
        _gold += amount;
        OnGoldChanged?.Invoke(this, _gold);
    }

    public void RemoveGold(int amount)
    {
        _gold -= amount;
        OnGoldChanged?.Invoke(this, _gold);
    }

    public int GetGoldOwned()
    {
        return _gold;
    }
}
