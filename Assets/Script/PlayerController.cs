using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Reference")]
    public InputManager iM;
    public Transform cameraTransform;
    public Rigidbody playerRigidbody;

    [Header("Movement")]
    public Vector3 moveDir;
    public float moveSpeed =7;
    public float rotationSpeed =7;

    public void Update()
    {
        
    }
    public void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }
    public void HandleMovement()
    {
        moveDir = cameraTransform.forward * iM.getMoveDir().y;
        moveDir += cameraTransform.right * iM.getMoveDir().x;
        moveDir.Normalize();
        moveDir.y = 0;
        Vector3 movementVelocity = moveDir * moveSpeed;
        playerRigidbody.velocity = movementVelocity;
    }
    public void HandleRotation()
    {
        Vector3 targetDir = Vector3.zero;
        targetDir = cameraTransform.forward * iM.getMoveDir().y;
        targetDir += cameraTransform.right * iM.getMoveDir().x;
        targetDir.Normalize();
        targetDir.y = 0;
        if (targetDir == Vector3.zero)
            targetDir = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        Quaternion playerRot = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRot;
    }
}
