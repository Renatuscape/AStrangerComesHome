using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public CameraController camController;
    public float timer = 0;
    public float threshold = 0.01f; // Small value to determine when the camera has reached the target
    float timerTick = 0.3f;
    float cameraMovementSpeed = 12;
    Transform camTransform;
    bool runningCoroutine;
    private void Start()
    {
        camTransform = camController.virtualCamera.transform;
    }
    private void Update()
    {
        if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView != CameraView.Normal)
        {
            // HandleCameraMovement();

            timer += Time.deltaTime;

            if (timer >= timerTick)
            {
                timer = 0;
                StopAllCoroutines();
                StartCoroutine(MoveCameraCoroutine());
            }
        }
        else if (TransientDataScript.CameraView == CameraView.Normal && runningCoroutine)
        {
            StopMovement();
        }
    }

    public void StopMovement()
    {
        StopAllCoroutines();
        runningCoroutine = false;
        camController.CameraNormal();
    }

    IEnumerator MoveCameraCoroutine()
    {
        runningCoroutine = true;
        var targetTransform = camController.targetTransform;
        Vector3 camPosition = camTransform.position;
        Vector3 targetPosition = targetTransform.position;
        targetPosition.z = camPosition.z; // Ensure Z is aligned

        float distance = Vector3.Distance(camPosition, targetPosition);
        float duration = distance / cameraMovementSpeed; // Calculate duration based on distance and speed

        float elapsedTime = 0f;

        while (distance > threshold)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // Progress from 0 to 1
            camTransform.position = Vector3.Lerp(camPosition, targetPosition, t);

            // Recalculate the distance
            distance = Vector3.Distance(camTransform.position, targetPosition);

            // Yield to wait for the next frame
            yield return null;
        }

        // Ensure the final position is exactly the target position
        camTransform.position = targetPosition;
        runningCoroutine = false;
    }
}
