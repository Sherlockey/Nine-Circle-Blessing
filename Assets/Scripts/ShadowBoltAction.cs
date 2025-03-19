using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBoltAction : MonoBehaviour, IAction
{
    public event EventHandler<bool> OnOnClick;

    [SerializeField] AbilityUpgrade _abilityUpgrade;

    private float _actionAttackScalar = 1f;
    private float _twinnedShadowAttackScalar = 0.5f;
    private bool _doesSearchForTarget = true;
    private float _chaosInShadowManaRestorePercentage = 0.25f;
    private float _tormentInShadowCooldownRestorePercentage = 0.25f;
    private float _growingDarkScalar = 0.25f;
    private bool _canPopupStack = false;

    delegate void OnHit();
    private OnHit _onHit;

    private void Start()
    {
        _abilityUpgrade.OnAbilityUpgradeSet += AbilityUpgrade_OnAbilityUpgradeSet;
        _abilityUpgrade.OnAbilityUpgradeReverted += AbilityUpgrade_OnAbilityUpgradeReverted;
    }

    private void AbilityUpgrade_OnAbilityUpgradeReverted(object sender, string upgradeName)
    {
        switch (upgradeName)
        {
            case "Twinned Shadows":
                _onHit = null;
                _canPopupStack = false;
                break;
            case "Chaos In Shadow":
                _onHit = null;
                break;
            case "Torment In Shadow":
                _onHit = null;
                break;
            case "Growing Dark":
                RevertGrowingDarkEffect();
                break;
            default:
                break;
        }
    }

    private void AbilityUpgrade_OnAbilityUpgradeSet(object sender, string upgradeName)
    {
        switch (upgradeName)
        {
            case "Twinned Shadows":
                _onHit += TwinnedShadowsEffect;
                _canPopupStack = true;
                break;
            case "Chaos In Shadow":
                _onHit += ChaosInShadowEffect;
                break;
            case "Torment In Shadow":
                _onHit += TormentInShadowEffect;
                break;
            case "Growing Dark":
                GrowingDarkEffect();
                break;
            default:
                break;
        }
    }

    public void OnClick()
    {
        OnOnClick?.Invoke(this, _doesSearchForTarget);

        BattleManager.Instance.Player.GetComponent<CharacterBattle>().SetSelectedAction(this);
    }

    public void Execute(CharacterBattle sourceCharacterBattle, CharacterBattle targetCharacterBattle)
    {
        sourceCharacterBattle.Attack(GetComponent<Stats>(), targetCharacterBattle, _actionAttackScalar, _canPopupStack);
        
        if (_onHit != null)
        {
            _onHit();
        }

        AudioManager.Instance.PlaySoundEffect("shadow_bolt", 0.25f);
    }

    public void PayCost()
    {
        return;
    }

    private void TwinnedShadowsEffect()
    {
        CharacterBattle playerCharacterBattle = GetComponent<CharacterBattle>();
        if (BattleManager.Instance.EnemyList.Count > 0)
        {
            GameObject secondaryTarget = BattleManager.Instance.EnemyList[UnityEngine.Random.Range(0, BattleManager.Instance.EnemyList.Count)];
            playerCharacterBattle.Attack(GetComponent<Stats>(), secondaryTarget.GetComponent<CharacterBattle>(), _actionAttackScalar * _twinnedShadowAttackScalar, _canPopupStack);
        }
    }

    private void ChaosInShadowEffect()
    {
        PlayerStats playerStats = GetComponent<PlayerStats>();
        float manaToRestore = playerStats.MaxMana * _chaosInShadowManaRestorePercentage;
        playerStats.RestoreMana(manaToRestore);
    }

    private void TormentInShadowEffect()
    {
        PlayerStats playerStats = GetComponent<PlayerStats>();
        float cooldownToRestore = playerStats.CooldownMax * _tormentInShadowCooldownRestorePercentage;
        playerStats.RestoreCooldown(cooldownToRestore);
    }

    private void GrowingDarkEffect()
    {
        _actionAttackScalar += _growingDarkScalar;
    }

    private void RevertGrowingDarkEffect()
    {
        _actionAttackScalar -= _growingDarkScalar;
    }
}
