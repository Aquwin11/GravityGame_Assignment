using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerInputs inputActions;

    public Vector2 movementValue;

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputs();
            inputActions.PlayerMovement.Movement.performed += i => movementValue = i.ReadValue<Vector2>();
        }
        inputActions.Enable();
    }
    public Vector2 getMoveDir()
    {
        return movementValue;
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
}
