using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronReflexesPact : Pact
{
    private float _evasionScalar = 0f;
    private float _armorScalar = 0.5f;

    public override void PactEffect()
    {
        PlayerStats playerStats = GetComponent<PlayerStats>();
        playerStats.EvasionScalar = _evasionScalar;
        playerStats.ArmorScalar += _armorScalar;
        playerStats.RefreshPlayerStats();
    }

    public override void RevertPactEffect()
    {
        PlayerStats playerStats = GetComponent<PlayerStats>();
        playerStats.EvasionScalar = 1;
        playerStats.ArmorScalar -= _armorScalar;
        playerStats.RefreshPlayerStats();
    }
}
