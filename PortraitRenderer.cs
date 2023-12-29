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
