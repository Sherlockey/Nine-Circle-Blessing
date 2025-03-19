using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthenedPact : Pact
{
    private PlayerStats _playerStats;
    private float _damageDealtIncrease = 0.15f;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
    }

    public override void PactEffect()
    {
        _playerStats.DamageDealtScalar += _damageDealtIncrease;
        _playerStats.RefreshPlayerStats();
    }

    public override void RevertPactEffect()
    {
        _playerStats.DamageDealtScalar -= _damageDealtIncrease;
        _playerStats.RefreshPlayerStats();
    }
}
