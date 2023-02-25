using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UIHoverShrinkButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    Image buttonImage = null;

    [SerializeField]
    float shrinkAmount = 0.9f;

    float growAmount = 1.05f;

    bool hovered = false;
    bool clickedOn = false;

    [SerializeField]
    UnityEvent clickEvent = null;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData != null)
        {
            if (eventData.button == 0)
            {
                clickedOn = true;
                buttonImage.rectTransform.DOScale(shrinkAmount, 0.1f);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
        buttonImage.rectTransform.DOScale(growAmount, 0.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hovered)
        {
            buttonImage.rectTransform.DOScale(1, 0.1f);
        }
        hovered = false;
        clickedOn = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData != null && hovered && clickedOn)
        {
            if (eventData.button == 0)
            {
                clickedOn = false;
                buttonImage.rectTransform.DOKill();
                buttonImage.rectTransform.DOScale(growAmount, 0.1f).OnComplete(CompleteClick);
            }
        }
    }

    public void CompleteClick()
    {
        AudioManager.instance.PlaySound(Sounds.MenuClick);

        if(clickEvent == null)
        {
            Debug.LogWarning("NO CLICK EVENT SET FOR " + gameObject.name);
            return;
        }

        clickEvent.Invoke();
        buttonImage.rectTransform.DOKill();
        buttonImage.rectTransform.DOScale(1, 0.1f);
    }
}
