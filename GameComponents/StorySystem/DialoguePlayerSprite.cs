using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePlayerSprite : MonoBehaviour
{
    public PlayerSprite playerSprite;
    public CharacterExpressionCatalogue expressionCatalogue;
    public List<Image> images = new();
    public List<Image> referenceImages = new();
    public Image imgExpression;
    public Image imgBrowColour;
    public Image imgLipTint;

    private void OnEnable()
    {
        HardReset();
    }

    public void HardReset()
    {
        Clear();
        CopyAllChildren();
    }
    public void CycleExpressions()
    {
        if (expressionCatalogue != null)
        {
            SetExpression(expressionCatalogue.GetNextPackageByIndex(false));
        }
        else
        {
            Debug.Log("Expression Catalogue was null. Set reference.");
        }
    }

    public void RefreshAllImages()
    {
        foreach (Image image in images)
        {
            var referenceImage = referenceImages.FirstOrDefault(i => i.name == image.name);

            if (referenceImage != null)
            {
                image.sprite = referenceImage.sprite;

                if (image.gameObject.activeInHierarchy != referenceImage.gameObject.activeInHierarchy)
                {
                    image.gameObject.SetActive(referenceImage.gameObject.activeInHierarchy);
                }
            }
        }
    }

    public void SetExpressionToDefault()
    {
        SetExpression(expressionCatalogue.GetPackageByID("DEFAULT"));
    }

    public void SetExpression(string expressionID)
    {
        var package = expressionCatalogue.GetPackageByID(expressionID);
        if (package != null)
        {
            SetExpression(package);
        }
        else
        {
            SetExpressionToDefault();
        }
    }
    public void SetExpression(PlayerExpressionPackage package)
    {
        //Image expression = images.FirstOrDefault(i => i.name.Contains("Expression"));
        //Image lipTint = images.FirstOrDefault(i => i.name.Contains("LipTint"));
        //Image browColour = images.FirstOrDefault(i => i.name.Contains("BrowColour"));

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            Debug.Log("Dialogue player sprite was not active. Attempting to activate.");
        }
        if (imgExpression != null)
        {
            imgExpression.sprite = package.expression;
            playerSprite.expression.sprite = imgExpression.sprite;
        }
        else
        {
            Debug.Log("Expression Image was not found in list.");
        }

        if (imgLipTint != null)
        {
            Color color = imgLipTint.color;
            imgLipTint.sprite = package.lipTint;
            imgLipTint.color = color;
            playerSprite.lipTint.sprite = imgLipTint.sprite;
        }
        else
        {
            Debug.Log("LipTint Image was not found in list.");
        }

        if (imgBrowColour != null)
        {
            Color color = imgBrowColour.color;
            imgBrowColour.sprite = package.browColour;
            imgBrowColour.color = color;
            playerSprite.playerHair.browColour.sprite = imgBrowColour.sprite;
        }
        else
        {
            Debug.Log("BrowColour Image was not found in list.");
        }
    }

    void CopyAllChildren()
    {
        foreach (Transform child in playerSprite.gameObject.transform)
        {
            referenceImages.Add(child.GetComponent<Image>());

            GameObject copiedChild = Instantiate(child.gameObject);
            copiedChild.transform.SetParent(gameObject.transform);

            copiedChild.transform.localPosition = child.localPosition;
            copiedChild.transform.localScale = new Vector3(1, 1, 1);

            copiedChild.name = child.name;

            if (copiedChild.name.Contains("Expression"))
            {
                imgExpression = copiedChild.GetComponent<Image>();
            }
            else if (copiedChild.name.Contains("LipTint"))
            {
                imgLipTint = copiedChild.GetComponent<Image>();
            }
            else if (copiedChild.name.Contains("BrowColour"))
            {
                imgBrowColour = copiedChild.GetComponent<Image>();
            }

            images.Add(copiedChild.GetComponent<Image>());
        }
    }

    private void OnDisable()
    {
        Clear();
    }

    void Clear()
    {
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        images.Clear();
        referenceImages.Clear();
    }
}
