using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlchemyAnimator : MonoBehaviour
{
    public List<Sprite> anim_create;
    public List<Sprite> anim_work;
    public List<Sprite> anim_complete;
    public Canvas animationCanvas;
    public Image image;

    public SynthesiserData synthData;
    public int frameIndex;
    public void Initialise(SynthesiserData synthData)
    {
        this.synthData = synthData;

        if (synthData.isSynthActive)
        {
            AnimateLoopWork();
        }
        else
        {
            animationCanvas.gameObject.SetActive(false);
        }
    }

    public void AnimateCreate()
    {
        StartCoroutine(Create());
    }

    void AnimateLoopWork()
    {
        if (synthData != null)
        {
            StartCoroutine(WorkLoop());
        }
    }

    IEnumerator Complete()
    {
        if (synthData != null)
        {
            animationCanvas.gameObject.SetActive(true);
            frameIndex = 0;

            if (anim_work != null && anim_work.Count > 0)
            {
                while (frameIndex < anim_create.Count)
                {
                    image.sprite = anim_create[frameIndex];
                    frameIndex++;
                    yield return new WaitForSeconds(0.1f);
                }
            }

            animationCanvas.gameObject.SetActive(false);
        }
    }

    IEnumerator WorkLoop()
    {
        Debug.Log("Started work loop");
        animationCanvas.gameObject.SetActive(true);
        frameIndex = 0;

        while (synthData.isSynthActive && synthData.progressSynth < synthData.synthRecipe.workload)
        {
            if (anim_work != null && anim_work.Count > 0)
            {
                image.sprite = anim_create[frameIndex];
                frameIndex++;

                if (frameIndex >= anim_create.Count)
                {
                    frameIndex = 0;
                }
            }
            else
            {
                image.sprite = anim_create[anim_create.Count - 1];
            }

            yield return new WaitForSeconds(0.1f);
        }

        StartCoroutine(Complete());
    }

    IEnumerator Create()
    {
        animationCanvas.gameObject.SetActive(true);

        frameIndex = 0;

        while (frameIndex < anim_create.Count)
        {
            image.sprite = anim_create[frameIndex];
            frameIndex++;
            yield return new WaitForSeconds(0.1f);
        }

        AnimateLoopWork();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        animationCanvas.gameObject.SetActive(false);
    }
}
