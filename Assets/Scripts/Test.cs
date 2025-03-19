using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GearInventory GearInventory;
    public AbilityUpgrade AbilityUpgrade;
    public PactInventory PactInventory;
    public Pact Pact;
    public TrinketInventory TrinketInventory;
    public Trinket Trinket;
    public MetaInventory MetaInventory;

    private void Start()
    {
        GearInventory = GameObject.Find("Velfirith(Clone)").GetComponent<GearInventory>();
        AbilityUpgrade = GameObject.Find("Velfirith(Clone)").GetComponent<AbilityUpgrade>();
        PactInventory = GameObject.Find("Velfirith(Clone)").GetComponent<PactInventory>();
        Pact = GameObject.Find("Velfirith(Clone)").GetComponent<PactInventory>().GetPactFromPactArrayByString("Covered");
        TrinketInventory = GameObject.Find("Velfirith(Clone)").GetComponent<TrinketInventory>();
        Trinket = TrinketInventory.GetTrinketFromTrinketArrayByString("Culling");
        MetaInventory = GameObject.Find("Velfirith(Clone)").GetComponent<MetaInventory>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GearGenerator.Instance.GenerateGear();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            GearInventory.SafeEquipGear(GearInventory.GearList[GearInventory.GearList.Count - 1]);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            GearInventory.UnequipGear(GearInventory.EquippedGearList[GearInventory.EquippedGearList.Count - 1], true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AbilityUpgrade.SetAbilityUpgradeEnabled("Twinned Shadows", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AbilityUpgrade.SetAbilityUpgradeEnabled("Chaos In Shadow", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AbilityUpgrade.SetAbilityUpgradeEnabled("Torment In Shadow", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AbilityUpgrade.SetAbilityUpgradeEnabled("Growing Dark", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            AbilityUpgrade.SetAbilityUpgradeEnabled("Twinned Chaos", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            AbilityUpgrade.SetAbilityUpgradeEnabled("Chaos Ensues", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            AbilityUpgrade.SetAbilityUpgradeEnabled("Entropy Feast", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            AbilityUpgrade.SetAbilityUpgradeEnabled("Pandemonium", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            AbilityUpgrade.SetAbilityUpgradeEnabled("Tormentor's Thirst", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            AbilityUpgrade.SetAbilityUpgradeEnabled("Concentrated Storm", true);
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            AbilityUpgrade.SetAbilityUpgradeEnabled("Hellfire", true);
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            AbilityUpgrade.SetAbilityUpgradeEnabled("First Rain", true);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            PactInventory.EnablePact(Pact, true);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            TrinketInventory.SetTrinketEnabled(Trinket, true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            MetaInventory.AddGold(1000);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            AudioManager.Instance.PlayMusic("the_heron");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            AudioManager.Instance.PlaySoundEffect("shadow_bolt");
        }
    }
}
