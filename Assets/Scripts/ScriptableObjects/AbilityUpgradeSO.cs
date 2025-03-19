using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AbilityUpgradeSO : ScriptableObject
{
    public string AbilityName;
    public string UpgradeName;
    public string Description;
    public bool Enabled;
}
