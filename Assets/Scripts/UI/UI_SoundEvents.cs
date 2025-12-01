using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SoundEvents : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Button btn = GetComponent<Button>();
        if (btn != null && btn.interactable)
        {
            if (AudioManager.instance != null)
                AudioManager.instance.PlayHover();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Button btn = GetComponent<Button>();
        if (btn != null && btn.interactable)
        {
            if (AudioManager.instance != null)
                AudioManager.instance.PlayClick();
        }
    }
}