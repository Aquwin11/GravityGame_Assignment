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
    public Transform gravityHologram;
    public Transform startPos;

    [Header("Movement")]
    public Vector3 moveDir;
    public float moveSpeed = 7;
    public float rotationSpeed = 7;
    private float moveAmount;//blendTree Value

    [Header("Falling&Jumping")]
    public bool isGrounded;
    public bool isJumped;
    public float jumpHeight;
    public float inAirTimer;
    public float Gravity; // Represents the magnitude of gravity
    public float fallingSpeed; // gravity multiplier
    public LayerMask groundLayer;
    public float rayCastOffset = 1;

    [Header("Gravity Manipulation")]
    public Vector3 currentGravityDirection = Vector3.down;
    public Vector3 newGravityDirection = Vector3.down;
    public float gravityRotationSpeed = 50f;
    public bool wasGrounded;
    public float detachDistance = 0.5f;

    [Header("HologramParameter")]//Rotation Reference based on the transform local rotation
    public Vector3 fRot;
    public Vector3 bRot;
    public Vector3 rRot;
    public Vector3 lRot;

    private void Start()
    {
        inAirTimer = Gravity;
        Physics.gravity = currentGravityDirection * Gravity;
        transform.position = startPos.position;
        transform.rotation = startPos.rotation;
        currentGravityDirection = -Vector3.up;
    }
    public void Update()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(iM.getMoveDir().x) + Mathf.Abs(iM.getMoveDir().y));//animation Blend value
        animManager.HandleAnimatorValues(0, moveAmount);

        isGrounded = CheckIfGrounded(); 

        if (wasGrounded && isGrounded) //Snap To ground if when standing on new surface
        {
            //SnapAlignWithSurface();
            Debug.Log("Snap Align player");
        }

        CheckIfJumped(); //Jump Input
        HandleGravitySelection(); // preform gravity 
        //CheckSurfaceNormal(); //Log Check if player is stand correctly to player
    }
    public void FixedUpdate()
    {
        //PhysicsCalculation
        HandleFalling();
        HandleRotation();
        HandleMovement();
        HandleJump();
        
    }
    private void LateUpdate()
    {
        cameraManager.HandleAllCameraFunction();
    }


    public void HandleGravitySelection()
    {
        //newGravity based on arrow key
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            newGravityDirection = transform.forward;
            SetHologramRotation(fRot);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            newGravityDirection = -transform.forward;
            SetHologramRotation(bRot);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            newGravityDirection = -transform.right;
            SetHologramRotation(lRot);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            newGravityDirection = transform.right;
            SetHologramRotation(rRot);
        }
        //Apply gravitiy after pressing enter
        if (Input.GetKeyDown(KeyCode.Return) && newGravityDirection.normalized != currentGravityDirection.normalized)
        {
            ApplyNewGravity(newGravityDirection);
        }
    }

    private void ApplyNewGravity(Vector3 newDir)
    {
        if (newDir == Vector3.zero)
            return;
        

        newDir.Normalize(); 
        // negative value are present because newDir is a localDirection of player 
        Vector3 abs = new Vector3(Mathf.Abs(newDir.x), Mathf.Abs(newDir.y), Mathf.Abs(newDir.z));
        
        //change gravity to one of the axis instead of spliting it between different axis
        if (abs.x > abs.y && abs.x > abs.z)
        {
            newDir = new Vector3(Mathf.Sign(newDir.x), 0f, 0f);         
        }
        else if (abs.y > abs.z)
        {
            newDir = new Vector3(0f, Mathf.Sign(newDir.y), 0f);         
        }
        else
        {
            newDir = new Vector3(0f, 0f, Mathf.Sign(newDir.z));         
        }

        

        currentGravityDirection = newDir;

        //Physics gravity change
        Physics.gravity = currentGravityDirection * Gravity;


        Vector3 newUpVector = -currentGravityDirection; // already unit-length

        if (isGrounded)
        {
            RaycastHit hit;
            Vector3 raycastOrigin = transform.position;
            raycastOrigin += -currentGravityDirection * rayCastOffset;

            if (Physics.SphereCast(
                raycastOrigin,
                0.15f,
                currentGravityDirection,
                out hit,
                rayCastOffset * 2f,
                groundLayer))
            {
                // If you want to *align to ground slope*, keep this:
                newUpVector = hit.normal.normalized;

            }
        }

        

        if (gravityHologram != null)
        {
            gravityHologram.gameObject.SetActive(false);
        }

        wasGrounded = true;
    }

    public void SetHologramRotation(Vector3 vector)
    {
        StopCoroutine(DisableHologram());
        gravityHologram.localRotation = Quaternion.Euler(vector);
        gravityHologram.gameObject.SetActive(true);
        StartCoroutine(DisableHologram());
    }

    IEnumerator DisableHologram()
    {
        yield return new WaitForSeconds(2f);
        if (gravityHologram != null)
        {
            gravityHologram.gameObject.SetActive(false);
        }
    }



    public void HandleMovement()
    {
        if (!isGrounded)
            return;
        //player movement based on the camera forward and right
        Vector3 cameraForwardProjected = Vector3.ProjectOnPlane(cameraTransform.forward, -currentGravityDirection);
        Vector3 cameraRightProjected = Vector3.ProjectOnPlane(cameraTransform.right, -currentGravityDirection);

        moveDir = cameraForwardProjected.normalized * iM.getMoveDir().y;
        moveDir += cameraRightProjected.normalized * iM.getMoveDir().x;

        moveDir.Normalize();
        moveDir = Vector3.ProjectOnPlane(moveDir, currentGravityDirection);

        Vector3 movementVelocity = moveDir * moveSpeed;
        Vector3 verticalVelocity = Vector3.Project(playerRigidbody.velocity, -currentGravityDirection);
        movementVelocity += verticalVelocity;

        playerRigidbody.velocity = movementVelocity;
    }

    public void HandleRotation()
    {
        Vector3 cameraForwardProjected = Vector3.ProjectOnPlane(cameraTransform.forward, -currentGravityDirection);
        Vector3 cameraRightProjected = Vector3.ProjectOnPlane(cameraTransform.right, -currentGravityDirection);

        Vector3 targetDir = Vector3.zero;
        targetDir = cameraForwardProjected.normalized * iM.getMoveDir().y;
        targetDir += cameraRightProjected.normalized * iM.getMoveDir().x;
        targetDir.Normalize();
        //Get target direction based on the camera forward and right

        targetDir = Vector3.ProjectOnPlane(targetDir, currentGravityDirection);

        if (targetDir == Vector3.zero)
            targetDir = Vector3.ProjectOnPlane(transform.forward, currentGravityDirection);

        Vector3 targetUpVector = -currentGravityDirection;

        if (isGrounded)
        {
            RaycastHit hit;
            Vector3 raycastOrigin = transform.position;
            raycastOrigin += -currentGravityDirection * rayCastOffset;

            if (Physics.SphereCast(raycastOrigin, 0.15f, currentGravityDirection, out hit, rayCastOffset * 2f, groundLayer))
            {
                targetUpVector = hit.normal;
                //get surface normal used for getting the up vector when rotating               
            }
        }
        //Rotate Player 
        Quaternion targetRotation = Quaternion.LookRotation(targetDir, targetUpVector);
        Quaternion playerRot = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRot;
    }

    public void HandleFalling()
    {
        animManager.SetIsFalling(!isGrounded);
        Vector3 gravityVector = currentGravityDirection * Gravity;

        if (!isGrounded && !isJumped)
        {
            inAirTimer += Time.deltaTime;
            playerRigidbody.AddForce(currentGravityDirection * fallingSpeed * inAirTimer);
        }
        else
        {
            inAirTimer = Gravity;
        }
    }
    private void CheckIfJumped()
    {
        if (iM.getJumpPressed() && isGrounded)
        {
            isJumped = true;
            Invoke("DisableJump", 0.3f);
        }
    }
    private void SnapAlignWithSurface()// Snap player first time landing on new surface 
    {
        if (!isGrounded) return;

        RaycastHit hit;
        Vector3 raycastOrigin = transform.position;
        if (Physics.Raycast(transform.position,currentGravityDirection, out hit, rayCastOffset * 2f,groundLayer ))
        {
            //Debug.Log("Hit " + hit.transform.name);
            Quaternion surfaceRot = Quaternion.FromToRotation(transform.up, hit.normal);
            Quaternion newRot = surfaceRot * Quaternion.AngleAxis(transform.rotation.eulerAngles.y, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRot, 50f);
        }
        wasGrounded = false;
    }

    public bool CheckIfGrounded()
    {
        RaycastHit hit;
        Vector3 raycastOrigin = transform.position;
        raycastOrigin += -currentGravityDirection * rayCastOffset;

        if (Physics.SphereCast(raycastOrigin, 0.15f, currentGravityDirection, out hit, rayCastOffset * 2f, groundLayer))
            return true;
        else
            return false;
    }

    public void HandleJump()
    {
        if (isJumped)
        {
            float JumpingVelocityMagnitude = Mathf.Sqrt(-2 * -Gravity * jumpHeight);
            Vector3 jumpVector = -currentGravityDirection * JumpingVelocityMagnitude;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(playerRigidbody.velocity, currentGravityDirection);
            playerRigidbody.velocity = horizontalVelocity + jumpVector;
        }
    }

    public void DisableJump()
    {
        isJumped = false;
    }

    private void OnDrawGizmos()
    {
        Vector3 raycastOrigin = transform.position;
        raycastOrigin += -currentGravityDirection * rayCastOffset;
        float radius = 0.15f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(raycastOrigin, radius);
        Gizmos.DrawLine(raycastOrigin, raycastOrigin + currentGravityDirection * rayCastOffset * 2f);

    }

    //Check if player standing is aligned 
    /*public void CheckSurfaceNormal()//
    {
        RaycastHit hit;
        Vector3 rayDirection = -transform.up; // Direction directly below the character

        if (Physics.Raycast(transform.position, currentGravityDirection, out hit, 10f, groundLayer))
        {
            float angle = Vector3.Angle(transform.up, hit.normal);
            if (angle > 18f)
            {
                
                
            }
        }
    }*/
    public int currentCoinCount;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEnter");
        // Check if the collided object has the "Coin" tag
        if (other.CompareTag("Coin"))
        {

            CollectCoin(1);

            Destroy(other.gameObject);
        }
    }

    private void CollectCoin(int value)
    {
        currentCoinCount += value;
        CoinEvents.OnCoinCollected.Invoke(currentCoinCount);
    }
}