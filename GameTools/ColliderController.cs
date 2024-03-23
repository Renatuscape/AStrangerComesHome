using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    public BoxCollider2D box;
    public CircleCollider2D circle;
    public CapsuleCollider2D capsule;

    public List<GameState> enabledStates;

    void Update()
    {
        CheckBox();
        CheckCircle();
        CheckCapsule();
    }

    private void CheckCapsule()
    {
        bool isEnabled = false;
        if (capsule != null)
        {
            foreach (var state in enabledStates)
            {
                if (TransientDataScript.GameState == state)
                {
                    isEnabled = true;
                    break;
                }
            }

            capsule.enabled = isEnabled;
        }
    }

    void CheckBox()
    {
        bool isEnabled = false;
        if (box != null)
        {
            foreach (var state in enabledStates)
            {
                if (TransientDataScript.GameState == state)
                {
                    isEnabled = true;
                    break;
                }
            }

            box.enabled = isEnabled;
        }
    }
    void CheckCircle()
    {
        bool isEnabled = false;
        if (circle != null)
        {
            foreach (var state in enabledStates)
            {
                if (TransientDataScript.GameState == state)
                {
                    isEnabled = true;
                    break;
                }
            }

            circle.enabled = isEnabled;
        }
    }
}
