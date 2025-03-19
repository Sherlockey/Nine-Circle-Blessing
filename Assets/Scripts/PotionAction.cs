using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionAction : MonoBehaviour, IAction
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
        float potionHealAmount = Mathf.Round(targetCharacterBattle.Stats.MaxHealth * BattleManager.Instance.Player.GetComponent<ItemInventory>().PotionSO.HealPercentage);

        sourceCharacterBattle.Heal(targetCharacterBattle, potionHealAmount, false);

        AudioManager.Instance.PlaySoundEffect("potion", 0.25f);
    }

    private bool EvaluateAction()
    {
        if (BattleManager.Instance.Player.GetComponent<ItemInventory>().PotionCount <= 0)
        {
            BattleUIManager.Instance.CreateCannotPerformActionPopup("Not\nEnough\nPotions");
            return false;
        }
        else if (GameManager.Instance.Player.GetComponent<Stats>().Health == GameManager.Instance.Player.GetComponent<Stats>().MaxHealth)
        {
            BattleUIManager.Instance.CreateCannotPerformActionPopup("Cannot use potion. Health is full");
            return false;
        }
        else
        {
            return true;
        }
    }

    public void PayCost()
    {
        BattleManager.Instance.Player.GetComponent<ItemInventory>().DecrementPotionCount();
    }
}
