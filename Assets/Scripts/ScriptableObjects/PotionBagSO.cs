using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PotionBagSO : ScriptableObject
{
    public Sprite Sprite;
    public string ObjectName;
    public string Description;
    public int PotionCapacity;
    public PotionSO PotionSO;
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
