using UnityEngine;

public class SatisfactionStockPrefab : MonoBehaviour
{
    public Item item;
    public ListItemPrefab listPrefab;

    public void UpdateValues()
    {
        if (item != null && listPrefab.entry != null && !string.IsNullOrEmpty(item.name))
        {
            if (listPrefab.entry.amount == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                //gameObject.SetActive(true);
                listPrefab.textMesh.text = $"{item.name} ({listPrefab.entry.amount})";
            }
        }
        else if (listPrefab.entry == null || listPrefab.entry.objectID != "ALERT")
        {
            gameObject.SetActive(false);
        }
    }
}
