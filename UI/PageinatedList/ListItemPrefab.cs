using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListItemPrefab: MonoBehaviour
{
    public IdIntPair entry;
    public TextMeshProUGUI textMesh;
    public Image bgImage;
    public Button button;
    public GameObject tagContainer;
    public GameObject tagPrefab;
    public Image spriteImage;

    public void CreateItemTags(Item item)
    {
        if (tagContainer != null && tagPrefab != null)
        {
            if (item != null && item.tagsArray != null)
            {
                foreach (var tag in item.tagsArray)
                {
                    var newTag = Instantiate(tagPrefab);

                    newTag.transform.SetParent(tagContainer.transform, false);

                    var formattedTag = tag[0].ToString().ToUpper() + tag.Remove(0, 1).ToLower();

                    newTag.GetComponentInChildren<TextMeshProUGUI>().text = formattedTag;
                }
            }
        }
        else
        {
            Debug.LogWarning("Tag container was null or tag prefab was null.");
        }
    }

    public void DisplayItemSprite(string spriteID)
    {
        if (spriteImage != null)
        {
            var sprite = SpriteFactory.GetItemSprite(spriteID);

            if (sprite != null)
            {
                spriteImage.sprite = sprite;
                spriteImage.gameObject.SetActive(true);
            }
            else
            {
                spriteImage.gameObject.SetActive(false);
            }
        }
    }
}
