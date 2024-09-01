using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    public TransientDataScript transientData;
    private readonly Dictionary<KeyCode, Action> keyBindings = new Dictionary<KeyCode, Action>();
    public CinemachineVirtualCamera virtualCamera;
    public Transform virtualCameraTransform;
    public Transform targetTransform;
    public GameObject camTargetX; //Exclude from loop
    public GameObject camTarget1;
    public GameObject camTarget2;
    public GameObject camTarget3;
    public GameObject camTargetGarden; //Garden
    public List<GameObject> camTargetsList;
    public int camIndex;

    public void CameraClose()
    {
        TransientDataScript.SetCameraView(CameraView.Cockpit);
        camIndex = 1;
        virtualCamera.m_Lens.OrthographicSize = 2;
        virtualCameraTransform.position = new Vector3(camTargetsList[camIndex].transform.position.x, camTargetsList[camIndex].transform.position.y, virtualCamera.gameObject.transform.position.z);
        HandleCameraView();
    }

    public void CameraNormal()
    {
        TransientDataScript.SetCameraView(CameraView.Normal);
        virtualCamera.m_Lens.OrthographicSize = 8.4f;
        virtualCameraTransform.position = new Vector3(0, 0, virtualCamera.gameObject.transform.position.z);
        HandleCameraView();
    }

    void Start()
    {
        InitializeKeyBindings();
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        virtualCameraTransform = virtualCamera.gameObject.transform;

        camTargetsList.Add(camTarget1);
        camTargetsList.Add(camTarget2);
        camTargetsList.Add(camTarget3);

        targetTransform = camTarget2.transform;

    }

    void Update()
    {
        if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView != CameraView.Normal)
        {
            ProcessInput();
        }
    }

    void InitializeKeyBindings()
    {
        keyBindings[KeyCode.RightArrow] = Right;
        keyBindings[KeyCode.D] = Right;
        keyBindings[KeyCode.LeftArrow] = Left;
        keyBindings[KeyCode.A] = Left;
        keyBindings[KeyCode.UpArrow] = () => { targetTransform = camTargetGarden.transform; TransientDataScript.SetCameraView(CameraView.Garden); };
        keyBindings[KeyCode.W] = () => { targetTransform = camTargetGarden.transform; TransientDataScript.SetCameraView(CameraView.Garden); };
        keyBindings[KeyCode.DownArrow] = () =>
        {
            camIndex = 0;
            targetTransform = camTargetX.transform;
        };
        keyBindings[KeyCode.S] = () =>
        {
            camIndex = 0;
            targetTransform = camTargetX.transform;
        };
    }

    void ProcessInput()
    {
        foreach (var keyBinding in keyBindings)
        {
            if (Input.GetKeyDown(keyBinding.Key))
            {
                keyBinding.Value.Invoke();
                break;
            }
        }
    }

    void Right()
    {
        if (targetTransform == camTargetX.transform)
            camIndex = 0;

        else if (targetTransform == camTargetGarden.transform)
        {
            camIndex = 0;
        }
        else
        {
            if (camIndex > 0)
                camIndex--;
            else
                camIndex = camTargetsList.Count - 1;
        }

        targetTransform = camTargetsList[camIndex].transform;
    }

    void Left()
    {
        if (targetTransform == camTargetGarden.transform)
        {
            camIndex = 1;
        }
        else
        {
            if (camIndex < camTargetsList.Count - 1)
                camIndex++;
            else
                camIndex = 0;
        }
        targetTransform = camTargetsList[camIndex].transform;
    }

    void HandleCameraView()
    {
        // Snap camera back to normal if the state is anything but these exceptions
        if (TransientDataScript.GameState != GameState.Overworld &&
            TransientDataScript.GameState != GameState.JournalMenu &&
            TransientDataScript.GameState != GameState.AlchemyMenu)
        {
            TransientDataScript.SetCameraView(CameraView.Normal);
            virtualCameraTransform.position = new Vector3(0, 0, virtualCamera.gameObject.transform.position.z);
        }

        else if (TransientDataScript.GameState == GameState.Overworld)
        {

            if (TransientDataScript.CameraView != CameraView.Normal)
            {
                if (targetTransform == camTargetGarden.transform)
                    TransientDataScript.SetCameraView(CameraView.Garden);
                else if (targetTransform == camTargetX.transform)
                    TransientDataScript.SetCameraView(CameraView.Cockpit);
                else if (targetTransform == camTarget1.transform)
                    TransientDataScript.SetCameraView(CameraView.Lounge);
                else if (targetTransform == camTarget2.transform)
                    TransientDataScript.SetCameraView(CameraView.Cockpit);
                else if (targetTransform == camTarget3.transform)
                    TransientDataScript.SetCameraView(CameraView.Pet);
            }
        }
    }
}
