using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PotionSO : ScriptableObject
{
    public GameObject Prefab;
    public Sprite Sprite;
    public string ObjectName;
    public float HealPercentage;
}
