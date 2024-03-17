using UnityEngine;

public class SecretAlchemyPrefab : MonoBehaviour
{
    public Recipe recipe;
    public int level;

    public void Initialise()
    {
        if (!string.IsNullOrEmpty(recipe.objectID))
        {
            level = Player.GetCount(recipe.objectID, name + recipe.objectID );
        }
    }
}