using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingRage : Trinket
{
    private PlayerStats _playerStats;
    private float _growingRageDamageIncrease = 0.01f;

    protected override void Awake()
    {
        base.Awake();
        _playerStats = GetComponent<PlayerStats>();
    }

    protected override void InitializeTrinket()
    {
        //do the effect immediately, and then register it do do the effect every time a battle is started and the encounter number is 1
        TrinketEffect();
        if (BattleManager.Instance != null)
        {
            BattleManager.OnBattleStarted += BattleManager_OnBattleStarted;
        }
    }

    private void BattleManager_OnBattleStarted(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.CurrentEncounterNumber == 1)
        {
            TrinketEffect();
        }
    }

    public override void TrinketEffect()
    {
        _playerStats.DamageScalar += _growingRageDamageIncrease;
    }
}
