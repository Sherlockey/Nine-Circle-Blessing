using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    public event EventHandler<int> OnPotionCountChanged;
    public event EventHandler<int> OnExplosiveCountChanged;

    private PotionBagSO _potionBagSO;
    private ExplosiveBagSO _explosiveBagSO;
    public PotionSO PotionSO { get; private set; }
    public ExplosiveSO ExplosiveSO { get; private set; }
    public int PotionCount { get; private set; }
    public int ExplosiveCount { get; private set; }

    private void Awake()
    {
        BattleManager.OnBattleStarted += BattleManager_OnBattleStarted; ;
    }

    private void BattleManager_OnBattleStarted(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.CurrentCircleNumber == 1 && GameManager.Instance.CurrentEncounterNumber == 1)
        {
            InitializePotions();
            InitializeExplosives();
        }
        else if (GameManager.Instance.CurrentEncounterNumber == 1)
        {
            HandlePotionRefresh();
            HandleExplosiveRefresh();
        }
    }

    public void SetPotionBagSO(PotionBagSO potionBagSO)
    {
        _potionBagSO = potionBagSO;
        potionBagSO.IsOwned = true;
    }

    public PotionBagSO GetPotionBagSO()
    {
        return _potionBagSO;
    }

    public void SetExplosiveBagSO(ExplosiveBagSO explosiveBagSO)
    {
        _explosiveBagSO = explosiveBagSO;
        explosiveBagSO.IsOwned = true;
    }

    public ExplosiveBagSO GetExplosiveBagSO()
    {
        return _explosiveBagSO;
    }

    private void InitializePotions()
    {
        if (_potionBagSO != null)
        {
            PotionSO = _potionBagSO.PotionSO;
            PotionCount = _potionBagSO.PotionCapacity;
            OnPotionCountChanged?.Invoke(this, PotionCount);
        }
    }

    private void InitializeExplosives()
    {
        if (_explosiveBagSO != null)
        {
            ExplosiveSO = Instantiate(_explosiveBagSO.ExplosiveSO);
            ExplosiveCount = _explosiveBagSO.ExplosiveCapacity;
            OnExplosiveCountChanged?.Invoke(this, ExplosiveCount);
        }
    }

    public void DecrementPotionCount()
    {
        PotionCount--;
        OnPotionCountChanged?.Invoke(this, PotionCount);
    }

    public void DecrementExplosiveCount()
    {
        ExplosiveCount--;
        OnExplosiveCountChanged?.Invoke(this, ExplosiveCount);
    }

    private void HandlePotionRefresh()
    {
        if (_potionBagSO == null)
        {
            return;
        }

        switch (_potionBagSO.BagRecoveryType)
        {
            case PotionBagSO.RecoveryType.None:
                break;
            case PotionBagSO.RecoveryType.OnePerCircle:
                RestorePotionUse(1);
                break;
            case PotionBagSO.RecoveryType.AllPerCircle:
                RestorePotionUse(_potionBagSO.PotionCapacity);
                break;
        }
    }

    private void HandleExplosiveRefresh()
    {
        if (_explosiveBagSO == null)
        {
            return;
        }

        switch (_explosiveBagSO.BagRecoveryType)
        {
            case ExplosiveBagSO.RecoveryType.None:
                break;
            case ExplosiveBagSO.RecoveryType.OnePerCircle:
                RestoreExplosiveUse(1);
                break;
            case ExplosiveBagSO.RecoveryType.AllPerCircle:
                RestoreExplosiveUse(_potionBagSO.PotionCapacity);
                break;
        }
    }

    private void RestorePotionUse(int restoreAmount)
    {
        PotionCount += restoreAmount;

        if (PotionCount > _potionBagSO.PotionCapacity)
        {
            PotionCount = _potionBagSO.PotionCapacity;
        }
        OnPotionCountChanged?.Invoke(this, PotionCount);
    }

    private void RestoreExplosiveUse(int restoreAmount)
    {
        ExplosiveCount += restoreAmount;

        if (ExplosiveCount > _explosiveBagSO.ExplosiveCapacity)
        {
            ExplosiveCount = _explosiveBagSO.ExplosiveCapacity;
        }
        OnExplosiveCountChanged?.Invoke(this, ExplosiveCount);
    }

    private void OnDestroy()
    {
        BattleManager.OnBattleStarted -= BattleManager_OnBattleStarted;
    }
}
