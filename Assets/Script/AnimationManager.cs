using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    Animator playerAnimator;

    [Header("AnimationParameters")]
    int horizontal;
    int vertical;
    int isFalling;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();

        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
        isFalling = Animator.StringToHash("IsFalling");
    }
    public void HandleAnimatorValues(float horizontalMovement, float verticalMovement)
    {
        playerAnimator.SetFloat(horizontal,horizontalMovement,0.1f,Time.deltaTime);
        playerAnimator.SetFloat(vertical,verticalMovement,0.1f,Time.deltaTime);
    }

    public void SetIsFalling(bool status)
    {
        playerAnimator.SetBool(isFalling,status);
    }
}
