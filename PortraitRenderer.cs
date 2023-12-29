using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitRenderer : MonoBehaviour
{
    public GameObject playerSprite;
    float playerDefaultX = -300f;
    public GameObject rightPortraitContainer;
    public Image rightCharacterImage;
    float rightDefaultX = 300f;
    public GameObject leftPortraitContainer;
    public Image leftCharacterImage;
    float leftDefaultX = -300f;
    bool isAnyPortraitActive;

    private void Awake()
    {
        ResetValues();
    }

    private void Update()
    {
        if (TransientDataScript.GameState == GameState.Overworld && isAnyPortraitActive)
        {
            ResetValues();
            isAnyPortraitActive = false;
        }
    }
    public void EnableForShop(string shopkeeperId)
    {
        //SET SPRITE
        SetSprite(rightCharacterImage, shopkeeperId);

        //ARRANGE
        isAnyPortraitActive = true;
        playerSprite.SetActive(true);
        MoveSprite(playerSprite, -370f);
        rightPortraitContainer.SetActive(true);
        MoveSprite(rightPortraitContainer, 370f);
    }

    public void EnableForTopicMenu(string topicMasterId)
    {
        //SET SPRITE
        SetSprite(rightCharacterImage, topicMasterId);

        //ARRANGE
        isAnyPortraitActive = true;
        playerSprite.SetActive(true);
        MoveSprite(playerSprite, -250f);
        rightPortraitContainer.SetActive(true);
        MoveSprite(rightPortraitContainer, 250);
    }
    
    public void EnableForDialogue()
    {
        ResetValues();

        //ARRANGE
        isAnyPortraitActive = true;
        playerSprite.SetActive(true);
        MoveSprite(playerSprite, -325);
        //rightPortraitContainer.SetActive(true);
        MoveSprite(rightPortraitContainer, 325);
    }

    //For other scripts to set the NPC portrait
    public void SetRightSprite(Character character, bool animate)
    {
        //Player sprite cannot be displayed here, so make sure to filter that away
        if (character.dialogueTag != "Traveller")
        {
            if (rightCharacterImage.sprite != character.sprite && animate)
            {
                AnimatePing(rightCharacterImage.gameObject);
            }
            rightPortraitContainer.SetActive(true);
            rightCharacterImage.sprite = character.sprite;
        }
    }

    void SetSprite(Image image, string characterId)
    {
        Character c = Characters.FindByID(characterId);

        if (c is not null)
        {
            image.sprite = c.sprite;
        }
        else
        {
            Debug.Log($"Character ID {characterId} not found. Using default.");
            c = Characters.FindByID("UNI000-SBQ");
            if (c is null)
            {
                Debug.Log($"Default character not found. Possible load order issue?");
            }
            else
            {
                image.sprite = c.sprite;
            }
        }
    }

    void MoveSprite(GameObject spriteObject, float positionX, bool transition = true)
    {
        if (transition == true)
        {
        StartCoroutine(MoveTransition(spriteObject.transform, positionX));
        }
        else
        {
            Transform portrait = spriteObject.transform;
            portrait.localPosition = new Vector3(positionX, portrait.localPosition.y, portrait.localPosition.z);
        }
    }

    IEnumerator MoveTransition(Transform portrait, float targetX, float timer = 0.001f)
    {
        float adjustment = 2f;

        if (portrait.localPosition.x > targetX + adjustment || portrait.localPosition.x < targetX - adjustment)
        {
            yield return new WaitForSeconds(timer);

            if (portrait.localPosition.x > targetX)
            {
                adjustment = adjustment * -1;
            }

            portrait.localPosition = new Vector3(portrait.localPosition.x + adjustment, portrait.localPosition.y, portrait.localPosition.z);

            StartCoroutine(MoveTransition(portrait, targetX, timer + 0.00015f));
        }
    }

    void AnimatePing(GameObject container)
    {

        var imageTransform = container.GetComponent<RectTransform>();
        Vector3 originalMeasurements = imageTransform.localScale;
        float targetScale = imageTransform.localScale.x * 1.03f;
        float timer = 0.01f;
        float adjustment = 0.002f;

        StartCoroutine(Grow(imageTransform, targetScale, timer, adjustment, originalMeasurements));
    }

    IEnumerator Grow(RectTransform imageTransform, float targetScale, float timer, float adjustment, Vector3 originalMeasurements)
    {
        yield return new WaitForSeconds(timer);

        if (imageTransform.localScale.x < targetScale)
        {
            imageTransform.localScale = new Vector3(imageTransform.localScale.x + adjustment, imageTransform.localScale.y + adjustment, imageTransform.localScale.z);
            StartCoroutine(Grow(imageTransform, targetScale, timer, adjustment, originalMeasurements));
        }
        else if (imageTransform.localScale.x >= targetScale)
        {
            StartCoroutine(Shrink(imageTransform, timer, adjustment, originalMeasurements));
        }
    }

    IEnumerator Shrink(RectTransform imageTransform, float timer, float adjustment, Vector3 originalMeasurements)
    {
        yield return new WaitForSeconds(timer);
        if (imageTransform.localScale.x > originalMeasurements.x)
        {
            imageTransform.localScale = new Vector3(imageTransform.localScale.x - adjustment, imageTransform.localScale.y - adjustment, imageTransform.localScale.z);
            StartCoroutine(Shrink(imageTransform, timer, adjustment, originalMeasurements));
        }
        else
        {
            imageTransform.localScale = new Vector3(originalMeasurements.x, originalMeasurements.y, imageTransform.localScale.z);
        }
    }

    private void OnDisable()
    {
        ResetValues();
    }

    void ResetValues()
    {
        playerSprite.SetActive(false);
        MoveSprite(playerSprite, playerDefaultX, false);
        rightPortraitContainer.SetActive(false);
        MoveSprite(rightPortraitContainer, rightDefaultX, false);
        leftPortraitContainer.SetActive(false);
        MoveSprite(leftPortraitContainer, leftDefaultX, false);
    }
}
