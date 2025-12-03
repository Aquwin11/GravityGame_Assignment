using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerInputs inputActions;

    [SerializeField] private Vector2 movementValue;
    [SerializeField] private Vector2 cameraValue;
    [SerializeField] private bool JumpPressed;


    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputs();
            inputActions.PlayerMovement.Movement.performed += i => movementValue = i.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => cameraValue = i.ReadValue<Vector2>();
            inputActions.PlayerAction.Jump.performed += i => JumpPressed = true;
            inputActions.PlayerAction.Jump.canceled += i => JumpPressed = false;
        }
        inputActions.Enable();
    }
    public Vector2 getMoveDir()
    {
        return movementValue;
    }
    public Vector2 getCameraDir()
    {
        return cameraValue;
    }
    public bool getJumpPressed()
    {
        return JumpPressed;
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
}
