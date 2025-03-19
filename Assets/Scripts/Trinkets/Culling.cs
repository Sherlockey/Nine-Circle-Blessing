using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Culling : Trinket
{
    private PlayerStats _playerStats;
    private float _cullingDamageIncrease = 0.4f;

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
        _playerStats.SetDamagedTargetScalarIncrease(_playerStats.GetDamagedTargetScalarIncrease() + _cullingDamageIncrease);
    }
}
