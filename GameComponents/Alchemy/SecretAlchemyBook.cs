using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretAlchemyBook : MonoBehaviour
{
    public SecretAlchemy secretAlchemy;
    public GameObject recipeContainer;
    public List<GameObject> recipePrefab;
    public List<Recipe> recipes;
    public string bookID;
    public bool isUnlocked;
    public bool Initialise()
    {
        if (Player.GetCount(bookID, name + bookID) > 0)
        {
            isUnlocked = true;
            gameObject.SetActive(true);
        }
        else
        {
            isUnlocked = false;
            gameObject.SetActive(false);
        }

        return isUnlocked;
    }
}
