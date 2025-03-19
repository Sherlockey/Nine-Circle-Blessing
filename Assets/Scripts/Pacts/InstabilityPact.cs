using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstabilityPact : Pact
{
    private float _actionDamageScalar = 0.3f;

    private void BattleManager_OnBattleStarted(object sender, EventArgs e)
    {
        foreach (GameObject enemy in BattleManager.Instance.EnemyList)
        {
            enemy.GetComponent<Stats>().TakeNonLethalDamage(GetComponent<Stats>().Damage * _actionDamageScalar, false);
        }
    }

    public override void PactEffect()
    {
        BattleManager.OnBattleStarted += BattleManager_OnBattleStarted;
        //this was added to make sure it does its effect as soon as it is taken
        foreach (GameObject enemy in BattleManager.Instance.EnemyList)
        {
            enemy.GetComponent<Stats>().TakeNonLethalDamage(GetComponent<Stats>().Damage * _actionDamageScalar, false);
        }
    }

    public override void RevertPactEffect()
    {
        BattleManager.OnBattleStarted -= BattleManager_OnBattleStarted;
    }

    private void OnDestroy()
    {
        BattleManager.OnBattleStarted -= BattleManager_OnBattleStarted;
    }
}
