using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestInteractable : Interactable
{

    public Inventory inventory;

    [SerializeField]
    private Text interactableText;

    public override void Activate()
    {
        interactableText.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        interactableText.gameObject.SetActive(false);
        if (InventoryVisualizer.instance.openInventory == inventory)
        {
            InventoryVisualizer.instance.CloseInventory();
        }
    }

    public override void Interact()
    {
        if (InventoryVisualizer.instance.openInventory == inventory)
        {
            InventoryVisualizer.instance.CloseInventory();
        }
        else
        {
            InventoryVisualizer.instance.OpenInventory(inventory);
        }
    }
}
