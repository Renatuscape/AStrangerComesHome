using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
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

    public Sprite defaultFrame;
    public List<Sprite> npcFrames;
    public List<Sprite> playerFrames;
    public Image leftFrame;
    public Image rightFrame;
    public Image playerFrame;

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

    Sprite FindNpcFrame(Character character)
    {
        var frame = npcFrames.FirstOrDefault(f => f.name.Contains(character.objectID));

        if (frame == null)
        {
            return defaultFrame;
        }
        else
        {
            return frame;
        }
    }

    Sprite FindPlayerFrame()
    {
        if (playerFrames == null || playerFrames.Count  == 0)
        {
            return defaultFrame;
        }

        string frameID = TransientDataScript.gameManager.dataManager.frameID;
        Sprite frame = playerFrames.FirstOrDefault(f => f.name.Contains(frameID));
        
        if (frame == null)
        {
            return playerFrames[0];
        }
        else
        {
            return frame;
        }
    }

    public void EnableForGarage(string characterID)
    {
        rightFrame.enabled = false;
        playerFrame.enabled = false;
        SetSprite(characterID);
        EnablePlayer();

        gameObject.SetActive(true);

        MoveSprite(playerSprite, -340f);
        MoveSprite(rightPortraitContainer, 340f);
    }
    public void EnableForShop(string characterID)
    {
        //SET SPRITE
        SetSprite(characterID);

        //ARRANGE
        gameObject.SetActive(true);
        EnablePlayer();
        MoveSprite(playerSprite, -370f);
        rightPortraitContainer.SetActive(true);
        MoveSprite(rightPortraitContainer, 370f);
    }

    public void EnableForTopicMenu(string characterID)
    {
        Character c = Characters.FindByID(characterID);

        SetSprite(c);

        if (c.type != CharacterType.Generic && Player.GetCount(c.objectID, name) < 1)
        {
            rightPortraitContainer.SetActive(false);
        }
        else
        {
            rightPortraitContainer.SetActive(true);
        }

        gameObject.SetActive(true);
        EnablePlayer();
        MoveSprite(playerSprite, -250f);
        MoveSprite(rightPortraitContainer, 250);
    }

    public void EnableForDialogue()
    {
        ResetValues();

        //ARRANGE
        gameObject.SetActive(true);
        EnablePlayer();
        MoveSprite(playerSprite, -325);
        MoveSprite(rightPortraitContainer, 325);
    }

    public void EnableForGifting(Character character)
    {
        gameObject.SetActive(true);
        SetSprite(character.objectID);
        rightPortraitContainer.SetActive(true);
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

    void EnablePlayer()
    {
        playerSprite.SetActive(true);
        playerFrame.sprite = FindPlayerFrame();
    }

    void SetSprite(string characterID, bool isRight = true)
    {
        Character c = Characters.FindByID(characterID);

        SetSprite(c, isRight);
    }

    void SetSprite(Character character, bool isRight = true)
    {
        Image characterImage;
        Image frameImage;

        if (isRight)
        {
            rightPortraitContainer.SetActive(true);
            characterImage = rightCharacterImage;
            frameImage = rightFrame;
        }
        else
        {
            leftPortraitContainer.SetActive(true);
            characterImage = leftCharacterImage;
            frameImage = leftFrame;
        }

        characterImage.sprite = character.sprite;
        frameImage.sprite = FindNpcFrame(character);
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
        leftFrame.enabled = true;
        rightFrame.enabled = true;
        playerFrame.enabled = true;
        playerSprite.SetActive(false);
        MoveSprite(playerSprite, playerDefaultX, false);
        rightPortraitContainer.SetActive(false);
        MoveSprite(rightPortraitContainer, rightDefaultX, false);
        leftPortraitContainer.SetActive(false);
        MoveSprite(leftPortraitContainer, leftDefaultX, false);
    }
}
