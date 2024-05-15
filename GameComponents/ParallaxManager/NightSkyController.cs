using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NightSkyController : MonoBehaviour
{
    public DataManagerScript dataManager;
    public SpriteRenderer screen;
    public float timeOfDay;
    public float alphaValue;
    public int starsToSpawn;
    public GameObject starPrefab;
    public GameObject starContainer;
    public List<StarPrefab> starList = new();
    public int maxStars;
    public bool rolledForStars;

    void Start()
    {
        screen = GetComponent<SpriteRenderer>();
        dataManager = TransientDataScript.gameManager.dataManager;
        maxStars = Random.Range(10, 101);
    }

    public void UpdateNightSky()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            timeOfDay = dataManager.timeOfDay;
            alphaValue = 0;

            // Night
            if (timeOfDay < 0.3f || timeOfDay > 0.8f)
            {
                alphaValue = 1;
            }

            // Dawn
            else if (timeOfDay >= 0.3f && timeOfDay <= 0.4f)
            {
                alphaValue = Mathf.SmoothStep(1, 0, (timeOfDay - 0.3f) / 0.1f);

                if (!rolledForStars)
                {
                    maxStars = Random.Range(10, 101);
                    rolledForStars = true;
                }
            }

            // Dusk
            else if (timeOfDay >= 0.7f && timeOfDay <= 0.8f)
            {
                alphaValue = (timeOfDay - 0.7f) / 0.1f;

                if (rolledForStars)
                {
                    rolledForStars = false;
                }
            }

            screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, alphaValue);

            // Interpolate starsToSpawn based on alphaValue
            float interpolatedStars = Mathf.Lerp(0, maxStars, alphaValue); // Adjust max stars as needed
            starsToSpawn = (int)interpolatedStars;

            if (starList.Count < starsToSpawn)
            {
                var newStar = Instantiate(starPrefab);
                newStar.transform.SetParent(starContainer.transform, false);
                newStar.transform.localPosition = new Vector3(Random.Range(-14.8f, 14.8f), Random.Range(5.7f, 8.2f), 0);
                starList.Add(newStar.GetComponent<StarPrefab>());
            }
            else if (starList.Count > starsToSpawn)
            {
                var randomStar = starList[Random.Range(0, starList.Count - 1)];
                starList.Remove(randomStar);
                randomStar.PutOut();
            }
        }
    }

    public void StarFlicker()
    {
        if (starList.Count > 0)
        {
            foreach (var star in starList)
            {
                if (star.flicker && star.setUpComplete && !star.isPuttingOut)
                {
                    star.Flicker();
                }
            }
        }
    }

    private void OnDisable()
    {
        foreach (var star in starList)
        {
            Destroy(star.gameObject);
        }

        starList.Clear();
    }
}
