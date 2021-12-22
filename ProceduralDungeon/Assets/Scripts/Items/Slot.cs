using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Image itemImage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryVisualizer.instance.hoveringOver = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryVisualizer.instance.hoveringOver = null;
    }
}
