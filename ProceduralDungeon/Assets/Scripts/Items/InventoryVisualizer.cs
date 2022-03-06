using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryVisualizer : MonoBehaviour
{
    public static InventoryVisualizer instance;

    [SerializeField]
    private int playerSlots; // Poèet hráèových slotù

    [SerializeField]
    private Item[] startItems; // Startovní itemy

    [SerializeField]
    public List<ItemProbability> lootTable;

    [SerializeField]
    private Color selectedSlotColor, defaultSlotColor;

    [SerializeField]
    private GameObject slotPrefab;

    [SerializeField]
    private GameObject accessorySlotPrefab;

    [SerializeField]
    private GameObject playerSlotsParent, openSlotsParent;

    [SerializeField]
    private Image cursorFollower;

    [SerializeField]
    private SpriteRenderer handImage;

    public Inventory playerInventory;
    public int selectedSlot;
    public bool isInventoryOpen { get; private set; }

    public Item selectedItem
    {
        get
        {
            return playerInventory.slots[selectedSlot];
        }
    }

    private List<Slot> playerInvSlots;

    // WIP
    public Inventory openInventory;

    private List<Slot> openInvSlots;

    // Naètené sprity itemù
    private Dictionary<string, Sprite> loadedSprites; 
    private Dictionary<string, Sprite> loadedHandSprites;

    public Slot hoveringOver;
    private Item holding;

    private RectTransform textHolder;
    private Text header;
    private Text content;

    private void Awake()
    {
        instance = this;

        playerInvSlots = new List<Slot>();
        openInvSlots = new List<Slot>();
        loadedSprites = new Dictionary<string, Sprite>();
        loadedHandSprites = new Dictionary<string, Sprite>();

        textHolder = cursorFollower.transform.GetChild(0).GetComponent<RectTransform>();
        header = textHolder.transform.GetChild(0).GetComponent<Text>();
        content = textHolder.transform.GetChild(1).GetComponent<Text>();

        // Vytvoøí všechny sloty hráèova inventáøe
        for (int i = 0; i < playerSlots; i++)
        {
            GameObject slotObj;
            if (i == playerSlots - 1)
            {
                slotObj = Instantiate(accessorySlotPrefab, playerSlotsParent.transform);
            }
            else
            {
                slotObj = Instantiate(slotPrefab, playerSlotsParent.transform);
            }
            
            Slot slot = slotObj.GetComponent<Slot>();
            
            slot.itemImage.enabled = false;
            playerInvSlots.Add(slot);
        }

        // Vytvoøí všechny sloty otevøeného inventáøe
        for (int i = 0; i < 20; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, openSlotsParent.transform);
            Slot slot = slotObj.GetComponent<Slot>();
            slot.itemImage.enabled = false;
            openInvSlots.Add(slot);
        }
        openSlotsParent.SetActive(false);

        // Naloadování spritù
        Sprite[] sprites = Resources.LoadAll<Sprite>("ItemSprites/");
        foreach(Sprite sprite in sprites)
        {
            loadedSprites.Add(sprite.name, sprite);
        }
        Sprite[] handSprites = Resources.LoadAll<Sprite>("HandItemSprites/");
        foreach (Sprite sprite in handSprites)
        {
            loadedHandSprites.Add(sprite.name, sprite);
        }

        // Setup hráèova inventáøe (objektu)
        playerInventory = new Inventory(playerSlots);
        playerInventory.inventoryChanged += PlayerInventoryChanged;
        selectedSlot = 0;

        // Pøidání startovních itemù do hráèova inventáøe
        foreach (Item item in startItems) playerInventory.TryAddItem(item);
    }

    private void PlayerInventoryChanged(Inventory inv)
    {
        InventoryChanged(playerInvSlots, inv);
    }

    private void OpenInventoryChanged(Inventory inv)
    {
        InventoryChanged(openInvSlots, inv);
    }

    private void InventoryChanged(List<Slot> invSlots, Inventory inv)
    {
        // Projede všechny sloty a zobrazí itemy
        for (int i = 0; i < inv.slots.Length; i++)
        {
            if(i == selectedSlot && inv == playerInventory)
            {
                invSlots[i].GetComponent<Image>().color = selectedSlotColor;
                if (selectedItem != null) handImage.sprite = GetHandSpriteByName(selectedItem.systemName + "_hand");
                else handImage.sprite = null;
            }
            else
            {
                invSlots[i].GetComponent<Image>().color = defaultSlotColor;
            }
            if(inv.slots[i] == null)
            {
                invSlots[i].itemImage.enabled = false;
            }
            else
            {
                invSlots[i].itemImage.enabled = true;
                invSlots[i].itemImage.sprite = GetSpriteByName(inv.slots[i].systemName);
            }
        }
    }

    public void Update()
    {
        // otevøení/zavøení inv. pomocí tlaèítka
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (isInventoryOpen) CloseInventory();
            else OpenInventory();
        }

        cursorFollower.transform.position = Input.mousePosition;

        float pivotX = Input.mousePosition.x;
        if (pivotX > Screen.width / 2) pivotX = 1;
        else pivotX = 0;
        textHolder.pivot = new Vector2(pivotX, 1);

        // zobrazuje popisky u pøedmìtù
        if (hoveringOver != null && holding == null && isInventoryOpen)
        {
            textHolder.gameObject.SetActive(true);
            if (playerInvSlots.Contains(hoveringOver)) // hráèùv inventáø
            {
                Item item = playerInventory.slots[playerInvSlots.IndexOf(hoveringOver)];
                if(item != null)
                {
                    header.text = item.displayName;
                    content.text = item.lore;
                }
                else textHolder.gameObject.SetActive(false);
            }
            else if (openInvSlots.Contains(hoveringOver)) // jiný inventáø (bedna, ...)
            {
                Item item = openInventory.slots[openInvSlots.IndexOf(hoveringOver)];
                if (item != null)
                {
                    header.text = item.displayName;
                    content.text = item.lore;
                }
                else textHolder.gameObject.SetActive(false);
            }
        }
        else
        {
            textHolder.gameObject.SetActive(false);
        }
        if(Input.GetMouseButtonDown(0) && hoveringOver != null && isInventoryOpen)
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                ShiftClick(); // automatické prohození itemu z jednoho inventáøe do druhého
            }
            else
            {
                SwitchHoldingItemWithSlot(); // manuální prohození itemu z jednoho inventáøe do druhého
            }
        }

        // pøepínání mezi sloty
        if(Input.mouseScrollDelta.y != 0)
        {
            selectedSlot -= (int)Input.mouseScrollDelta.y;
            if (selectedSlot >= playerInventory.slots.Length - 1) selectedSlot = 0;
            if (selectedSlot < 0) selectedSlot = playerInventory.slots.Length - 2;
            PlayerInventoryChanged(playerInventory);
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            selectedSlot = 0;
            PlayerInventoryChanged(playerInventory);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            selectedSlot = 1;
            PlayerInventoryChanged(playerInventory);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            selectedSlot = 2;
            PlayerInventoryChanged(playerInventory);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            selectedSlot = 3;
            PlayerInventoryChanged(playerInventory);
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            selectedSlot = 4;
            PlayerInventoryChanged(playerInventory);
        }
        if (Input.GetKey(KeyCode.Alpha6))
        {
            selectedSlot = 5;
            PlayerInventoryChanged(playerInventory);
        }
        if (Input.GetKey(KeyCode.Alpha7))
        {
            selectedSlot = 6;
            PlayerInventoryChanged(playerInventory);
        }
        /*if (Input.GetKey(KeyCode.Alpha8))
        {
            selectedSlot = 7;
            PlayerInventoryChanged(playerInventory);
        }*/
    }

    private void ShiftClick() // automaticky pøesune item na který kliknu do druhého inventáøe
    {
        if (openInventory == null) return;
        if (playerInvSlots.Contains(hoveringOver)) // pokud je to hráèùv inventáø
        {
            Item i = playerInventory.SwitchSlot(playerInvSlots.IndexOf(hoveringOver), null);
            if(!openInventory.TryAddItem(i))
            {
                playerInventory.SwitchSlot(playerInvSlots.IndexOf(hoveringOver), i); // pokud nelze pøidat item do druhého inventáøe, vrátí ho zpìt na své místo
            }
        }
        else // pokud to není hráèùv inventáø
        {
            Item i = openInventory.SwitchSlot(openInvSlots.IndexOf(hoveringOver), null);
            if (!playerInventory.TryAddItem(i))
            {
                openInventory.SwitchSlot(openInvSlots.IndexOf(hoveringOver), i); // pokud nelze pøidat item do druhého inventáøe, vrátí ho zpìt na své místo
            }
            else
            {
                if(!(playerInventory.slots[7] is Accessory) && !(playerInventory.slots[7] == null))
                {
                    playerInventory.SwitchSlot(7, null);
                    openInventory.SwitchSlot(openInvSlots.IndexOf(hoveringOver), i);
                }
            }
        }
    }

    private void SwitchHoldingItemWithSlot() // manuální pøesouvání itemù
    {
        if (playerInvSlots.Contains(hoveringOver))
        {
            int selectedIndex = playerInvSlots.IndexOf(hoveringOver);
            if (selectedIndex == 7)
            {
                if(holding is Accessory || holding == null) holding = playerInventory.SwitchSlot(selectedIndex, holding);
            }
            else
            {
                holding = playerInventory.SwitchSlot(selectedIndex, holding);
            }
            
        }
        else
        {
            holding = openInventory.SwitchSlot(openInvSlots.IndexOf(hoveringOver), holding);
        }
        if (holding == null)
        {
            cursorFollower.enabled = false;
        }
        else
        {
            cursorFollower.enabled = true;
            cursorFollower.sprite = GetSpriteByName(holding.systemName);
        }
    }

    public Sprite GetSpriteByName(string name)
    {
        if (loadedSprites.ContainsKey(name))
        {
            return loadedSprites[name];
        }
        else
        {
            return loadedSprites["default"];
        }
    }

    public Sprite GetHandSpriteByName(string name)
    {
        if (loadedHandSprites.ContainsKey(name))
        {
            return loadedHandSprites[name];
        }
        else
        {
            return loadedHandSprites["default_hand"];
        }
    }

    // WIP
    public void OpenInventory(Inventory inv=null)
    {
        isInventoryOpen = true;
        playerSlotsParent.transform.localScale = Vector3.one;
        openInventory = inv;
        if(openInventory != null)
        {
            openInventory.inventoryChanged += OpenInventoryChanged;
            openSlotsParent.SetActive(true);
            for (int i = 0; i < 20; i++)
            {
                if (i < inv.slots.Length) openInvSlots[i].gameObject.SetActive(true);
                else openInvSlots[i].gameObject.SetActive(false);
            }
            OpenInventoryChanged(openInventory);
        }
    }

    public void CloseInventory()
    {
        // pokud hráè drží kurzorem pøedmìt a snaží se zavøít inventáø, nejdøíve se tato funkce pokusí vložit pøedmìt do hráèova inventáøe,
        // pokud to nejde, pokusí se ho vložit do otevøeného inventáøe (bedny, ...)
        if(holding != null)
        {
            if(!playerInventory.TryAddItem(holding))
            {
                openInventory.TryAddItem(holding);
            }
            holding = null;
            cursorFollower.enabled = false;
        }

        isInventoryOpen = false;
        playerSlotsParent.transform.localScale = 0.5f * Vector3.one;
        if (openInventory == null) return;
        openInventory.inventoryChanged -= OpenInventoryChanged;
        if (openInvSlots.Contains(hoveringOver)) hoveringOver = null;
        openInventory = null;
        openSlotsParent.SetActive(false);
    }
    
}
