using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public class AlchemyObject
{
    public string objectID;
    public ItemIntPair itemEntry;

    public AlchemyMenu alchemyMenu;

    public AlchemyInventoryItem inventoryClass;
    public TextMeshProUGUI inventoryAmountText;
    public GameObject inventoryContainer;
    public ItemType lastInventoryCategory;

    public List<AlchemyDraggableItem> draggableObjects = new();
    public GameObject infusionContainer;
    public GameObject materialContainer;

    public GameObject selectedEntryPrefab;
    public GameObject infusionList;
    public GameObject materialList;
    public GameObject dragParent;

    public int currentlyInBag;
    public int currentlyOnTable;
    public bool isMaterial;
    public bool isInfusion;

    public void Initialise(ItemIntPair itemEntry, AlchemyMenu alchemyMenu)
    {
        this.alchemyMenu = alchemyMenu;
        this.itemEntry = itemEntry;
        objectID = itemEntry.item.objectID;
        currentlyInBag = itemEntry.amount;

        infusionContainer = alchemyMenu.infusionContainer;
        materialContainer = alchemyMenu.materialContainer;
        inventoryContainer = alchemyMenu.inventoryContainer;
        dragParent = alchemyMenu.dragParent;
        infusionList = alchemyMenu.infusionList;
        materialList = alchemyMenu.materialList;

        //CreateInventoryPrefab();
    }

    //public void CheckInventoryDisplay(ItemType currentCategory, bool anyCategory = false)
    //{
    //    lastInventoryCategory = currentCategory;

    //    if ((itemEntry.item.type == currentCategory || anyCategory) && currentlyInBag > 0)
    //    {
    //        inventoryClass.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        inventoryClass.gameObject.SetActive(false);
    //    }
    //}

    //public void CreateInventoryPrefab()
    //{
    //    var itemIcon = BoxFactory.CreateItemIcon(itemEntry.item, true, 64, 18);
    //    itemIcon.gameObject.name = itemEntry.item.name;
    //    itemIcon.gameObject.transform.SetParent(inventoryContainer.transform, false);
    //    var script = itemIcon.gameObject.AddComponent<AlchemyInventoryItem>();
    //    script.item = itemEntry.item;
    //    script.alchemyObject = this;

    //    inventoryAmountText = itemIcon.countText;
    //    UpdateNumbers();

    //    inventoryClass = script;
    //}

    public void UpdateNumbers()
    {
        if (selectedEntryPrefab != null)
        {
            UnityEngine.Object.Destroy(selectedEntryPrefab);
        }

        if (currentlyOnTable > 0)
        {
            var newPrefab = BoxFactory.CreateItemRow(itemEntry.item, currentlyOnTable);
            newPrefab.name = itemEntry.item.name;


            if (isInfusion)
            {
                newPrefab.transform.SetParent(infusionList.transform, false); // FIX PARENT CONTAINER REFERENCE
            }
            else
            {
                newPrefab.transform.SetParent(materialList.transform, false); // FIX PARENT CONTAINER REFERENCE
            }

            var scaleAnimator = newPrefab.GetComponent<Anim_ScaleOnEnable>();
            scaleAnimator.startScale = new Vector3(0.9f, 1, 1);

            selectedEntryPrefab = newPrefab;
        }

        if (currentlyInBag == 0)
        {
            inventoryClass.gameObject.SetActive(false);
        }

        inventoryAmountText.text = $"{currentlyInBag}";
    }

    public void SetAsInfusion()
    {
        isInfusion = true;
        isMaterial = false;

        foreach (var draggable in draggableObjects)
        {
            draggable.gameObject.SetActive(true);
            draggable.gameObject.transform.SetParent(infusionContainer.transform);
        }

        UpdateNumbers();
    }

    public void SetAsMaterial()
    {
        isInfusion = false;
        isMaterial = true;

        foreach (var draggable in draggableObjects)
        {
            draggable.gameObject.SetActive(true);
            draggable.gameObject.transform.SetParent(materialContainer.transform);
        }

        UpdateNumbers();
    }

    public void TakeFromInventory()
    {
        currentlyInBag--;
        currentlyOnTable++;

        //Spawning draggable is done in (inventoryClass) AlchemyInventoryItem

        UpdateNumbers();
    }

    public void ReturnToInventory(AlchemyDraggableItem draggable)
    {
        draggableObjects.Remove(draggable);
        draggable.gameObject.GetComponent<ItemIconData>().Return("Alchemy object on return to inventory.");

        currentlyInBag++;
        currentlyOnTable--;

        UpdateNumbers();
    }

    public void UseForCreation(bool isDebugging = false) //only called on create
    {
        if (isDebugging)
        {
            Debug.Log($"Simulating create. \'Removing\' {currentlyOnTable} {itemEntry.item.name} from inventory.");
        }
        else
        {
            Player.Remove(objectID, currentlyOnTable);
        }

        foreach (var draggable in draggableObjects)
        {
            currentlyOnTable--;
            //UnityEngine.Object.Destroy(draggable.gameObject); // Handle destroy in the animation component
        }

        draggableObjects.Clear();
        UpdateNumbers();
    }

    internal GameObject SpawnDraggable()
    {
        TakeFromInventory();

        // ADD CHECKS FOR THE AMOUNT OF STUFF ALREADY SPAWNED

        var item = itemEntry.item;
        var prefab = BoxFactory.CreateItemIcon(item, false, 96).gameObject;
        prefab.transform.SetParent(dragParent.transform, false);
        prefab.name = item.name;
        prefab.AddComponent<AlchemyDraggableItem>();

        var script = prefab.GetComponent<AlchemyDraggableItem>();
        script.item = item;
        script.dragParent = dragParent;
        script.alchemyObject = this;
        draggableObjects.Add(script);

        UpdateNumbers();

        return prefab;
    }
}