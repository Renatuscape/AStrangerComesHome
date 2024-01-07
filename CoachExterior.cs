using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoachExterior : MonoBehaviour
{
    public Transform TopTarget;
    public Transform BottomTarget;
    public Transform LeftTarget;
    public Transform RightTarget;
    SpriteRenderer Renderer { get; set; }
    float FadeTick { get; set; } = 0.05f;
    float FadeIncrement { get; set; } = 0.2f;
    float Alpha { get; set; } = 1;

    bool isWorking;

    void Start()
    {
        Renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isWorking && TransientDataScript.GameState == GameState.Overworld)
        {
            var worldPosition = MouseTracker.GetMouseWorldPosition();

            if (worldPosition.x > LeftTarget.position.x
                && worldPosition.x < RightTarget.position.x
                && worldPosition.y < TopTarget.position.y
                && worldPosition.y > BottomTarget.position.y)
            {
                TryHideExterior();
            }
            else
            {
                TryCoverExterior();
            }
        }
    }

    void TryHideExterior()
    {
        if (!isWorking)
        {
            isWorking = true;
            StartCoroutine(HideExterior());
        }
        else
        {
            StopAllCoroutines();
            isWorking = false;
            TryHideExterior();
        }
    }

    void TryCoverExterior()
    {
        if (!isWorking)
        {
            isWorking = true;
            StartCoroutine(CoverExterior());
        }
        else
        {
            StopAllCoroutines();
            isWorking = false;
            TryCoverExterior();
        }
    }


    IEnumerator HideExterior()
    {
        while (Alpha > 0)
        {
            Alpha -= FadeIncrement;
            Renderer.color = new Color(1, 1, 1, Alpha);
            yield return new WaitForSeconds(FadeTick);
        }
        Alpha = 0;
        isWorking = false;
    }
    IEnumerator CoverExterior()
    {
        while (Alpha < 1)
        {
            Alpha += FadeIncrement;
            Renderer.color = new Color(1, 1, 1, Alpha);
            yield return new WaitForSeconds(FadeTick);
        }
        Alpha = 1;
        isWorking = false;
    }
}
