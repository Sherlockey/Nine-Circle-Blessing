using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoveredPact : Pact
{
    private PlayerStats _playerStats;
    private float _armorIncrease = 0.15f;
    private float _evasionIncrease = 0.15f;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
    }

    public override void PactEffect()
    {
        _playerStats.ArmorScalar += _armorIncrease;
        _playerStats.EvasionScalar += _evasionIncrease;
        _playerStats.RefreshPlayerStats();
    }

    public override void RevertPactEffect()
    {
        _playerStats.ArmorScalar -= _armorIncrease;
        _playerStats.EvasionScalar -= _evasionIncrease;
        _playerStats.RefreshPlayerStats();
    }
}
