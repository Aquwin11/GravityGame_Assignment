using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Reference")]
    public Transform playerTransform;
    public Transform cameraPivot;
    public InputManager inputManager;
    public Transform cameraTransform;

    [Header("CameraCollision")]
    public float defaultPosition;
    public float cameraCollisionRadius =0.2f;
    public LayerMask collisionLayer;
    public float cameraCollisionOffset = 0.2f;//how much the camera will jump of objects colliding with
    public float minimumCollisionOffset =0.2f;

    [Header("CameraFollow&RotationParameters")]
    public Vector3 cameraVectorPosition;
    public Vector3 cameraFollowVelocity = Vector3.zero;
    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 0.75f; //cameraSensitivity
    public float minRotation = -35f;
    public float maxRotation = 35f;
    public float lookAngle;//Camera Up&Down
    public float pivotAngle;//Camera Left&Right

    public void Start()
    {
        defaultPosition = cameraTransform.localPosition.z;
    }
    public void HandleAllCameraFunction()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollision();
    }

    private void FollowTarget()
    {

        Vector3 targetPos = Vector3.Lerp(transform.position, playerTransform.position, cameraFollowSpeed);
        transform.position = targetPos;
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;
        //{Change from previous Commit}:  forgot to add delta Time
        lookAngle += (inputManager.getCameraDir().x * cameraLookSpeed * Time.deltaTime);
        pivotAngle -= (inputManager.getCameraDir().y * cameraLookSpeed * Time.deltaTime);
        pivotAngle = Mathf.Clamp(pivotAngle, minRotation, maxRotation);
        /*rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;*/
        Quaternion gravityAlign = Quaternion.FromToRotation(Vector3.up, playerTransform.up);
        Quaternion yawRotation = Quaternion.AngleAxis(lookAngle, playerTransform.up);//rotate around playersY
        transform.rotation = Quaternion.Slerp(transform.rotation, yawRotation * gravityAlign, 0.2f);


        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }
    void HandleCameraCollision()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if(Physics.SphereCast
            (cameraPivot.transform.position,cameraCollisionRadius,direction, out hit,Mathf.Abs(targetPosition),collisionLayer))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = targetPosition - (distance - cameraCollisionOffset);
        }
        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
            targetPosition -= minimumCollisionOffset;
        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition,2.5f);
        cameraTransform.localPosition = cameraVectorPosition;
    }

    private void OnDrawGizmos()
    {
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(cameraTransform.position, cameraCollisionRadius * 5);
    }
}
