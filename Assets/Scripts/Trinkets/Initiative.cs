using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initiative : Trinket
{
    private PlayerStats _playerStats;
    private float _initiativeDamageIncrease = 0.4f;

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
        _playerStats.SetUndamagedTargetScalarIncrease(_playerStats.GetUndamagedTargetScalarIncrease() + _initiativeDamageIncrease);
    }
}
