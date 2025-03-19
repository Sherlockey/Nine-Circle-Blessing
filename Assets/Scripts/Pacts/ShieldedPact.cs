using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldedPact : Pact
{
    private PlayerStats _playerStats;
    private float _damageTakenReduction = 0.15f;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
    }

    public override void PactEffect()
    {
        _playerStats.DamageTakenScalar -= _damageTakenReduction;
        _playerStats.RefreshPlayerStats();
    }

    public override void RevertPactEffect()
    {
        _playerStats.DamageTakenScalar += _damageTakenReduction;
        _playerStats.RefreshPlayerStats();
    }
}
