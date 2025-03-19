using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;

    private Canvas _canvas;
    private Vector3 _lastSlottedPosition;

    private void Awake()
    {
        //TODO change this to be injected when the UIManager creates these DragDrop prefabs
        _canvas = GameObject.Find("BattleCanvas").GetComponent<Canvas>();
        GameManager.Instance.SceneAboutToBeChanged += GameManager_SceneAboutToBeChanged;
    }

    private void GameManager_SceneAboutToBeChanged(object sender, EventArgs e)
    {
        transform.localPosition = GetLastSlottedPosition();
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == null)
        {
            eventData.pointerDrag.transform.localPosition = eventData.pointerDrag.GetComponent<DragDrop>().GetLastSlottedPosition();
        }
        else if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Gear>() != null)
        {
            Drop(eventData);
        }
        else if (eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlot>() == null)
        {
            eventData.pointerDrag.transform.localPosition = eventData.pointerDrag.GetComponent<DragDrop>().GetLastSlottedPosition();
        }
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }

    private void Drop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }

        if (GetComponent<Gear>().ThisGearType.ToString() != eventData.pointerCurrentRaycast.gameObject.GetComponent<Gear>().ThisGearType.ToString())
        {
            eventData.pointerDrag.transform.localPosition = eventData.pointerDrag.GetComponent<DragDrop>().GetLastSlottedPosition();
        }
        else if (GetComponent<Gear>() != eventData.pointerCurrentRaycast.gameObject.GetComponent<Gear>())
        {
            eventData.pointerDrag.transform.SetParent(eventData.pointerDrag.transform.parent.parent.parent.GetChild(2).GetChild(0));

            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = eventData.pointerCurrentRaycast.gameObject.transform.GetComponent<RectTransform>().anchoredPosition;

            BattleManager.Instance.Player.GetComponent<GearInventory>().SafeEquipGear(eventData.pointerDrag.GetComponent<Gear>());

            BattleUIManager.Instance.OrganizeGear();

            Destroy(eventData.pointerDrag.GetComponent<DragDrop>());
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public Vector3 GetLastSlottedPosition()
    {
        return _lastSlottedPosition;
    }

    public void SetLastSlottedPosition(Vector3 localPosition)
    {
        _lastSlottedPosition = localPosition;
    }

    private void OnDestroy()
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;
        GameManager.Instance.SceneAboutToBeChanged -= GameManager_SceneAboutToBeChanged;
    }
}
