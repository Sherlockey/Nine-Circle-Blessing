using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreaterConstitution : Trinket
{
    private PlayerStats _playerStats;
    private float _healthIncreasePercentage = 0.1f;

    protected override void Awake()
    {
        base.Awake();
        _playerStats = GetComponent<PlayerStats>();
    }

    protected override void InitializeTrinket()
    {
        TrinketEffect();
    }


    public override void TrinketEffect()
    {
        _playerStats.MaxHealthScalar += _healthIncreasePercentage;
        _playerStats.RefreshPlayerStats();
    }
}
