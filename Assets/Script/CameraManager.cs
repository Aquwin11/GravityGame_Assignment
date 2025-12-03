using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform playerTransform;
    public Transform cameraPivot;
    public InputManager inputManager;

    public Vector3 cameraFollowVelocity = Vector3.zero;
    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 0.75f; //cameraSensitivity
    public float minRotation = -35f;
    public float maxRotation = 35f;
    public float lookAngle;//Camera Up&Down
    public float pivotAngle;//Camera Left&Right


    public void HandleAllCameraFunction()
    {
        FollowTarget();
        RotateCamera();
    }

    private void FollowTarget()
    {
        Vector3 targetPos = Vector3.SmoothDamp(transform.position, playerTransform.position,
            ref cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPos;
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;
        lookAngle += (inputManager.getCameraDir().x * cameraLookSpeed);
        pivotAngle -= (inputManager.getCameraDir().y * cameraLookSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minRotation, maxRotation);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

}
