using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SecretAlchemy : MonoBehaviour
{
    public SecretAlchemyBook book1;
    public SecretAlchemyBook book2;
    public SecretAlchemyBook book3;
    public SecretAlchemyBook book4;
    public SecretAlchemyBook book5;
    public Recipe activeRecipe;

    private void OnEnable()
    {
        CheckTheBooks();
    }

    void CheckTheBooks()
    {
        if (book1.Initialise())
        {
            if (book2.Initialise())
            {
                if (book3.Initialise())
                {
                    if (book4.Initialise())
                    {
                        book5.Initialise();
                    }
                }
            }

            FillPages();
        }
    }

    void FillPages()
    {
        foreach (Recipe rx in Recipes.all.Where(r => !r.notResearchable && !r.hidden))
        {
            if (book5.isUnlocked && rx.rarity >= ItemRarity.Mythical)
            {
                book5.recipes.Add(rx);
            }
            else if (book4.isUnlocked && rx.rarity >= ItemRarity.Extraordinary)
            {
                book4.recipes.Add(rx);
            }
            else if (book3.isUnlocked && rx.rarity >= ItemRarity.Rare)
            {
                book3.recipes.Add(rx);
            }
            else if (book2.isUnlocked && rx.rarity >= ItemRarity.Uncommon)
            {
                book2.recipes.Add(rx);
            }
            else if (book1.isUnlocked)
            {
                book1.recipes.Add(rx);
            }
        }
    }
}
