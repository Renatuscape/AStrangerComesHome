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

    private void Start()
    {
        ResetValues();
    }
    public void EnableForShop(Character shopkeeper)
    {
        //SET SPRITE
        SetSprite(rightCharacterImage, shopkeeper);

        //ARRANGE
        playerSprite.SetActive(true);
        playerSprite.transform.position = new Vector3(-370f, 0, 0);
        MoveSprite(playerSprite, -370f);
        rightPortraitContainer.SetActive(true);
        MoveSprite(rightPortraitContainer, 370f);
    }

    private void OnDisable()
    {
        ResetValues();
    }

    void ResetValues()
    {
        playerSprite.SetActive(false);
        MoveSprite(playerSprite, playerDefaultX);
        rightPortraitContainer.SetActive(false);
        MoveSprite(rightPortraitContainer, rightDefaultX);
        leftPortraitContainer.SetActive(false);
        MoveSprite(leftPortraitContainer, leftDefaultX);
    }

    void SetSprite(Image image, Character character)
    {
        image.sprite = character.sprite;
    }

    void MoveSprite(GameObject spriteObject, float positionX)
    {
        spriteObject.transform.localPosition = new Vector3(positionX, spriteObject.transform.localPosition.y, spriteObject.transform.localPosition.z);
    }
}
