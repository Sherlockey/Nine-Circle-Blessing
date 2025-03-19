using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HastedPact : Pact
{
    private PlayerStats _playerStats;
    private float _speedIncrease = 0.10f;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
    }

    public override void PactEffect()
    {
        _playerStats.SpeedScalar += _speedIncrease;
        _playerStats.RefreshPlayerStats();
    }

    public override void RevertPactEffect()
    {
        _playerStats.SpeedScalar -= _speedIncrease;
        _playerStats.RefreshPlayerStats();
    }
}
