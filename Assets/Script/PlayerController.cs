using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Reference")]
    public Transform cameraTransform;
    public Rigidbody playerRigidbody;
    public InputManager iM;
    public AnimationManager animManager;
    public CameraManager cameraManager;

    [Header("Movement")]
    public Vector3 moveDir;
    public float moveSpeed =7;
    public float rotationSpeed =7;
    private float moveAmount;

    [Header("Falling&Jumping")]
    public bool isGrounded;
    public bool isJumped;
    public float jumpHeight;
    public float inAirTimer;
    public float Gravity;
    public float fallingSpeed;
    public LayerMask groundLayer;

    private void Start()
    {
        inAirTimer = Gravity;
    }
    public void Update()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(iM.getMoveDir().x) + Mathf.Abs(iM.getMoveDir().y));
        animManager.HandleAnimatorValues(0,moveAmount);
        isGrounded = CheckIfGrounded();
        CheckIfJumped();
    }
    public void FixedUpdate()
    {
        HandleFalling();
        HandleRotation();
        HandleMovement();
        HandleJump();
    }
    private void LateUpdate()
    {
        cameraManager.HandleAllCameraFunction();
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

    public void HandleFalling()
    {
        animManager.SetIsFalling(!isGrounded);
        if(!isGrounded && !isJumped)
        {
            inAirTimer += Time.deltaTime;
            //playerRigidbody.AddForce(transform.forward * lerpingVelocity);
            playerRigidbody.AddForce(-Vector3.up * fallingSpeed * inAirTimer);
        }
        else
        {
            inAirTimer = Gravity;
        }
    }

    public bool CheckIfGrounded()
    {
        RaycastHit hit;
        Vector3 raycastOrigin = transform.position;
        raycastOrigin.y += 0.25f;
        if (Physics.SphereCast(raycastOrigin, 0.15f, -Vector3.up, out hit, groundLayer))
            return true;        
        else
            return false;

    }
    private void CheckIfJumped()
    {
        if (iM.getJumpPressed() && isGrounded)
        {
            isJumped = true;
            Invoke("DisableJump", 0.25f);
        }
    }
    public void HandleJump()
    {
        if (isJumped)
        {
            float JumpingVelocity = Mathf.Sqrt(-2 * -Gravity * jumpHeight) ;
            Vector3 playerVelocity = moveDir;
            playerVelocity.y = JumpingVelocity;
            playerRigidbody.velocity = playerVelocity;
        }
    }

    public void DisableJump()
    {
        isJumped = false;
    }
}
