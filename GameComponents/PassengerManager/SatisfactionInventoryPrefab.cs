using UnityEngine;

public class SatisfactionInventoryPrefab : MonoBehaviour
{
    public Item item;
    public ListItemPrefab listPrefab;

    public void UpdateValues()
    {
        if (item != null && !string.IsNullOrEmpty(item.name))
        {
            var count = Player.GetCount(item.objectID, name);

            if (count == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                listPrefab.textMesh.text = $"{item.name} ({Player.GetCount(item.objectID, name)})";
            }
        }
        else if (listPrefab.entry == null || listPrefab.entry.objectID != "ALERT")
        {
            gameObject.SetActive(false);
        }
    }
}