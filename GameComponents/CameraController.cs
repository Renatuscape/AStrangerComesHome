using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public TransientDataScript transientData;

    public CinemachineVirtualCamera vCam;
    public Transform camTransform;
    public Transform targetTransform;
    public GameObject camTargetX; //Exclude from loop
    public GameObject camTarget1;
    public GameObject camTarget2;
    public GameObject camTarget3;
    public GameObject camTargetGarden; //Garden
    public List<GameObject> camTargetsList;
    public int camIndex;
    public float cameraMovementSpeed = 0.2f;

    void Start()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        camTransform = vCam.gameObject.transform;

        camTargetsList.Add(camTarget1);
        camTargetsList.Add(camTarget2);
        camTargetsList.Add(camTarget3);

        targetTransform = camTarget2.transform;

    }
    private void Update()
    {
        HandleCameraView();
    }
    void FixedUpdate()
    {
        HandleCameraMovement();
    }

    void HandleCameraView()
    {
        if (TransientDataScript.CameraView == CameraView.Normal && (vCam.m_Lens.OrthographicSize != 7 || camTransform.position.x != 0))
            CameraNormal();

        if (TransientDataScript.GameState != GameState.Overworld && TransientDataScript.GameState != GameState.JournalMenu) //CAMERA ALWAYS SNAPS BACK TO NORMAL WHEN LEAVING OVERWORLD
        {
            TransientDataScript.SetCameraView(CameraView.Normal);
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


                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
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

                else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
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
                else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                {
                    targetTransform = camTargetGarden.transform;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                {
                    camIndex = 0;
                    targetTransform = camTargetX.transform;
                }

                /*if (vCam.gameObject.transform.position != camTargetsList[camIndex].transform.position)
                {
                    var camPosition = camTransform.position;
                    var targetPosition = targetTransform.transform.position;
                    var step = cameraMovementSpeed * Vector3.Distance(camPosition, targetPosition); //cushions movement
                    camTransform.position = Vector3.MoveTowards(camPosition, new Vector3(targetPosition.x, targetPosition.y, camPosition.z), step);
                }*/
            }
        }
    }
    void HandleCameraMovement()
    {
        if (vCam.gameObject.transform.position != camTargetsList[camIndex].transform.position)
        {
            var camPosition = camTransform.position;
            var targetPosition = targetTransform.transform.position;
            var step = cameraMovementSpeed * Vector3.Distance(camPosition, targetPosition); //cushions movement
            camTransform.position = Vector3.MoveTowards(camPosition, new Vector3(targetPosition.x, targetPosition.y, camPosition.z), step);
        }
    }

    public void CameraClose()
    {
        TransientDataScript.SetCameraView(CameraView.Cockpit);
        camIndex = 1;
        vCam.m_Lens.OrthographicSize = 2;
        camTransform.position = new Vector3(camTargetsList[camIndex].transform.position.x, camTargetsList[camIndex].transform.position.y, vCam.gameObject.transform.position.z);
    }

    public void CameraNormal()
    {
        TransientDataScript.SetCameraView(CameraView.Normal);
        vCam.m_Lens.OrthographicSize = 7;
        camTransform.position = new Vector3(0, 0, vCam.gameObject.transform.position.z);
    }
}
