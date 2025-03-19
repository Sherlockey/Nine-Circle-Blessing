using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveAction : MonoBehaviour, IAction
{
    public event EventHandler<bool> OnOnClick;

    private bool _doesSearchForTarget = false;
    
    public void OnClick()
    {
        if (EvaluateAction() == false)
        {
            return;
        }

        OnOnClick?.Invoke(this, _doesSearchForTarget);

        BattleManager.Instance.Player.GetComponent<CharacterBattle>().SetSelectedAction(this);
    }

    public void Execute(CharacterBattle sourceCharacterBattle, CharacterBattle targetCharacterBattle)
    {
        float damage = sourceCharacterBattle.Stats.Damage * BattleManager.Instance.Player.GetComponent<ItemInventory>().ExplosiveSO.DamageScalar;

        targetCharacterBattle.Stats.TakeDamage(GetComponent<Stats>(), damage, false);

        AudioManager.Instance.PlaySoundEffect("explosive", 0.25f);
    }

    private bool EvaluateAction()
    {
        if (BattleManager.Instance.Player.GetComponent<ItemInventory>().ExplosiveCount <= 0)
        {
            BattleUIManager.Instance.CreateCannotPerformActionPopup("Not\nEnough\nExplosives");
            return false;
        }
        else
        {
            return true;
        }
    }

    public void PayCost()
    {
        BattleManager.Instance.Player.GetComponent<ItemInventory>().DecrementExplosiveCount();
    }
}
