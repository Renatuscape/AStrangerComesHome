using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PortraitRenderer : MonoBehaviour
{
    public PlayerSprite playerSpriteData;
    public GameObject playerSprite;
    float playerDefaultX = -600f;
    public GameObject rightPortraitContainer;
    public Image rightCharacterImage;
    float rightDefaultX = 600f;
    public GameObject leftPortraitContainer;
    public Image leftCharacterImage;
    float leftDefaultX = -600f;

    private void Awake()
    {
        ResetValues();
    }

    private void Update()
    {
        if (TransientDataScript.GameState == GameState.Overworld)
        {
            ResetValues();
            gameObject.SetActive(false);
        }
    }

    public void UpdatePlayerSprite()
    {
        playerSpriteData.UpdateAllFromGameData(out var hair, out var eyes);
    }

    public void EnableForGarage(string characterID)
    {
        SetSprite(characterID);
        EnablePlayer();

        gameObject.SetActive(true);

        MoveSprite(playerSprite, -645f);
        MoveSprite(rightPortraitContainer, 675f);
    }
    public void EnableForShop(string characterID)
    {

        //SET SPRITE
        SetSprite(characterID);

        //ARRANGE
        gameObject.SetActive(true);
        playerSprite.transform.localPosition = new Vector3(-1020, playerSprite.transform.localPosition.y, 0);
        rightPortraitContainer.transform.localPosition = new Vector3(1020, playerSprite.transform.localPosition.y, 0);
        EnablePlayer();
        MoveSprite(playerSprite, -630);
        rightPortraitContainer.SetActive(true);
        MoveSprite(rightPortraitContainer, 630);
    }

    public void EnableForTopicMenu(string characterID)
    {
        Character c = Characters.FindByID(characterID);

        SetSprite(c);

        rightPortraitContainer.SetActive(true);

        gameObject.SetActive(true);
        EnablePlayer();
        MoveSprite(playerSprite, -500f);
        MoveSprite(rightPortraitContainer, 500);
    }

    public void EnableForGifting(Character character)
    {
        gameObject.SetActive(true);
        SetSprite(character.objectID);
        rightPortraitContainer.SetActive(true);
        MoveSprite(rightPortraitContainer, 650);
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

    void EnablePlayer()
    {
        playerSprite.SetActive(true);
    }

    void SetSprite(string characterID, bool isRight = true)
    {
        Character c = Characters.FindByID(characterID);

        SetSprite(c, isRight);
    }

    void SetSprite(Character character, bool isRight = true)
    {
        Image characterImage;

        if (isRight)
        {
            rightPortraitContainer.SetActive(true);
            characterImage = rightCharacterImage;
        }
        else
        {
            leftPortraitContainer.SetActive(true);
            characterImage = leftCharacterImage;
        }

        characterImage.sprite = character.sprite;
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

    IEnumerator MoveTransition(Transform portrait, float targetX)
    {
        float duration = 0.5f; // Adjust the duration as needed
        float timer = 0f;
        Vector3 startPosition = portrait.localPosition;
        Vector3 targetPosition = new Vector3(targetX, portrait.localPosition.y, portrait.localPosition.z);

        while (timer < duration)
        {
            float t = timer / duration;
            portrait.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            timer += Time.deltaTime;
            yield return null;
        }

        portrait.localPosition = targetPosition;
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
