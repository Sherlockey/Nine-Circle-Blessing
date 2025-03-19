using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ExplosiveBagSO : ScriptableObject
{
    public Sprite Sprite;
    public string ObjectName;
    public string Description;
    public int ExplosiveCapacity;
    public ExplosiveSO ExplosiveSO;
    public RecoveryType BagRecoveryType;
    public bool IsOwned;
    public int Cost;
    public int Rank;

    public enum RecoveryType
    {
        None,
        OnePerCircle,
        AllPerCircle,
    }
}
