using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public event EventHandler OnGearEquipped;

    [SerializeField] private GameObject _itemTooltipPrefab;
    [SerializeField] private SlotType _gearType;

    private static readonly Vector2 EQUIPMENT_POSITION = new Vector2(300, -40);

    private GameObject _canvas;
    private GameObject _itemTooltip;

    private void Start()
    {
        _canvas = GameObject.Find("BattleCanvas");
    }

    public enum SlotType
    {
        Inventory,
        MainHand,
        OffHand,
        Helmet,
        BodyArmor,
        Ring,
        Trinket,
        Potion,
        Explosive,
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null )
        {
            return;
        }
        //check if the pointerDrag game object's Gear.GearType matches the slotType by string
        if (_gearType.ToString() != eventData.pointerDrag.GetComponent<Gear>().ThisGearType.ToString())
        {
            //if not, reset the position
            eventData.pointerDrag.transform.localPosition = eventData.pointerDrag.GetComponent<DragDrop>().GetLastSlottedPosition();
        }
        else
        {
            //if so, change parent, move it, destroy the DragDrop component, and equip it
            eventData.pointerDrag.transform.SetParent(eventData.pointerDrag.transform.parent.parent.parent.GetChild(2).GetChild(0));

            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

            Destroy(eventData.pointerDrag.GetComponent<DragDrop>());

            BattleManager.Instance.Player.GetComponent<GearInventory>().SafeEquipGear(eventData.pointerDrag.GetComponent<Gear>());

            //Send a message to the UI so the inventory can be sorted again
            OnGearEquipped?.Invoke(this, EventArgs.Empty);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_itemTooltip != null)
        {
            return;
        }
        
        switch (_gearType)
        {
            case SlotType.Trinket:
                if (GameManager.Instance.Player.GetComponent<TrinketInventory>().GetEquippedTrinket() == null)
                {
                    return;
                }
                GenerateTrinketTooltip();
                break;
            case SlotType.Potion:
                if (GameManager.Instance.Player.GetComponent<ItemInventory>().GetPotionBagSO() == null)
                {
                    return;
                }
                GeneratePotionBagTooltip();
                break;
            case SlotType.Explosive:
                if (GameManager.Instance.Player.GetComponent<ItemInventory>().GetExplosiveBagSO() == null)
                {
                    return;
                }
                GenerateExplosiveBagTooltip();
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(_itemTooltip);
    }

    private void GenerateTrinketTooltip()
    {
        _itemTooltip = Instantiate(_itemTooltipPrefab, _canvas.transform);
        _itemTooltip.GetComponent<RectTransform>().localPosition = EQUIPMENT_POSITION;

        _itemTooltip.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Trinket of " + GameManager.Instance.Player.GetComponent<TrinketInventory>().GetEquippedTrinket().GetSuffix();
        _itemTooltip.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = GameManager.Instance.Player.GetComponent<TrinketInventory>().GetEquippedTrinket().GetDescription();
    }

    private void GeneratePotionBagTooltip()
    {
        _itemTooltip = Instantiate(_itemTooltipPrefab, _canvas.transform);
        _itemTooltip.GetComponent<RectTransform>().localPosition = EQUIPMENT_POSITION;

        _itemTooltip.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = GameManager.Instance.Player.GetComponent<ItemInventory>().GetPotionBagSO().ObjectName;
        _itemTooltip.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = GameManager.Instance.Player.GetComponent<ItemInventory>().GetPotionBagSO().Description;
    }

    private void GenerateExplosiveBagTooltip()
    {
        _itemTooltip = Instantiate(_itemTooltipPrefab, _canvas.transform);
        _itemTooltip.GetComponent<RectTransform>().localPosition = EQUIPMENT_POSITION;

        ExplosiveBagSO explosiveBagSO = GameManager.Instance.Player.GetComponent<ItemInventory>().GetExplosiveBagSO();
        _itemTooltip.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = explosiveBagSO.ObjectName;
        _itemTooltip.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = explosiveBagSO.Description;
    }
}
