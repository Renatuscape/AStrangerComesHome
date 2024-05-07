using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum JournalMainPage
{
    Inventory,
    Statistics,
    Quests,
    Collections
}

public enum StatisticPage
{
    Abilities,
    Skills,
    Upgrades
}
public enum QuestPage
{
    Ongoing,
    Complete,
    Main,
    Romance
}

public class Journal : MonoBehaviour
{
    public JournalMainPage mainPage;
    public FontManager fontManager;

    public CardFloatAnimator inventoryAnimator;
    public CardFloatAnimator statisticsAnimator;
    public CardFloatAnimator questsAnimator;
    public CardFloatAnimator collectionsAnimator;

    public List<GameObject> containers;

    public List<GameObject> pageContent;

    public List<GameObject> targets;

    public List<CardFloatAnimator> cards;

    public int mainPageIndex = 0;
    public int statPageIndex = 0;
    public int questPageIndex = 0;
    public int collectionPageIndex = 0;

    void Start()
    {
        inventoryAnimator.positionIndex = 0;
        cards.Add(inventoryAnimator);

        statisticsAnimator.positionIndex = 1;
        cards.Add(statisticsAnimator);

        questsAnimator.positionIndex = 2;
        cards.Add(questsAnimator);

        collectionsAnimator.positionIndex = 3;
        cards.Add(collectionsAnimator);

        foreach (var card in cards)
        {
            // Access the TextMeshPro objects within the child components of each card
            TMP_Text textMesh = card.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault();
            textMesh.font = fontManager.subtitle.font;
        }
    }

    private void OnEnable()
    {
        AudioManager.PlayUISound("bookPlace1");
        foreach (var card in cards)
        {
            // Access the TextMeshPro objects within the child components of each card
            TMP_Text textMesh = card.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault();
            textMesh.font = fontManager.subtitle.font;
        }
    }

    private void OnDisable()
    {
        AudioManager.PlayUISound("bookPlace1");
    }

    private void Update()
    {
        if (TransientDataScript.GameState == GameState.JournalMenu)
        {
            if (mainPage == JournalMainPage.Inventory)
            {
                pageContent[0].SetActive(true);
                pageContent[1].SetActive(false);
                pageContent[2].SetActive(false);
                pageContent[3].SetActive(false);
            }
            else if (mainPage == JournalMainPage.Statistics)
            {
                pageContent[0].SetActive(false);
                pageContent[1].SetActive(true);
                pageContent[2].SetActive(false);
                pageContent[3].SetActive(false);
            }
            else if (mainPage == JournalMainPage.Quests)
            {
                pageContent[0].SetActive(false);
                pageContent[1].SetActive(false);
                pageContent[2].SetActive(true);
                pageContent[3].SetActive(false);
            }
            else if (mainPage == JournalMainPage.Collections)
            {
                pageContent[0].SetActive(false);
                pageContent[1].SetActive(false);
                pageContent[2].SetActive(false);
                pageContent[3].SetActive(true);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void MainPageCycle()
    {
        mainPageIndex++;
        if (mainPageIndex >= System.Enum.GetValues(typeof(JournalMainPage)).Length)
        {
            mainPageIndex = 0;
        }
        mainPage = (JournalMainPage)mainPageIndex;
        
        AudioManager.PlayUISound("bookFlip2");

        foreach (var card in cards)
        {
            card.IncrementIndex();

            StartCoroutine(MoveCards(card, targets[card.positionIndex]));
        }
    }

    IEnumerator MoveCards(CardFloatAnimator card, GameObject target)
    {
        card.transform.parent.transform.SetParent(containers[card.positionIndex].transform);
        Transform cardParentTransform = card.gameObject.transform.parent;
        Transform targetTransform = target.transform;

        Vector3 startPosition = cardParentTransform.position;
        Vector3 targetPosition = targetTransform.position;

        float duration = 0.3f; // Adjust this value to change the duration of the movement
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            cardParentTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            card.outline.transform.position = card.transform.position;
            yield return null; // Wait for the next frame
        }

        // Ensure the card parent reaches exactly the target position
        cardParentTransform.position = targetPosition;
        yield return null;
    }
}
