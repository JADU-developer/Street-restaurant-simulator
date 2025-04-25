using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private enum PlayerCurrentState
    {
        Walking
    }

    [SerializeField] private PlayerCurrentState playerCurrentState;
    private Vector3 moveDirection;

    [Header("Reference")]
    [SerializeField] private Transform PlayerCamera;
    private CharacterController characterController;

    [Header("Movement")]
    [SerializeField, Range(1, 20)] private float fWalkSpeed = 20f;
    private float fMoveSpeed;
    private Vector3 velocity;

    [Header("Speed Multiplier")]
    [SerializeField, Range(0.1f, 1f)] private float fGroundMultiplier = 1f;
    [SerializeField, Range(0.1f, 1f)] private float fAirMultiplier = 0.7f;
    private float fSpeedMultiplier;

    [Header("Gravity")]
    [SerializeField, Range(-0.1f, -20)] private float fGravity = -9.8f;
    [SerializeField, Range(0.5f, 2f)] private float fGroundCheckDistance = 1.5f;
    [SerializeField] private LayerMask WhatIsGround;
    private bool IsGrounded;

    [Header("Audio")]
    [SerializeField] private AudioClip[] FootStepSound;
    [SerializeField] private float FoodstepSOundPlayDelay = 0.1f;
    [SerializeField] private float Volume = 1f;

    private bool isPlayingFootstepSound = false;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        StartCoroutine(CheckIsGrounded());
    }

    private void Update()
    {
        AddGravity();
        RotatePlayer();
        PlayerInput();
        UpdatePlayerState();
    }

    private void PlayerInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection = moveDirection.normalized;

        if (moveDirection.x != 0 && moveDirection.z != 0)
        {
            Move();
        }
    }

    private void UpdatePlayerState()
    {
        playerCurrentState = PlayerCurrentState.Walking;
        fMoveSpeed = fWalkSpeed;
    }

    private void RotatePlayer()
    {
        float cameraYRotation = PlayerCamera.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, cameraYRotation, 0);
    }

    private IEnumerator CheckIsGrounded()
    {
        while (true)
        {
            if (Physics.Raycast(transform.position, Vector3.down, fGroundCheckDistance, WhatIsGround))
            {
                IsGrounded = true;
                fSpeedMultiplier = fGroundMultiplier;
            }
            else
            {
                IsGrounded = false;
                fSpeedMultiplier = fAirMultiplier;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void AddGravity()
    {
        if (!IsGrounded)
        {
            velocity.y += fGravity * Time.deltaTime;
        }
        else
        {
            velocity.y = -2f;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    private void Move()
    {
        characterController.Move(moveDirection * fSpeedMultiplier * fMoveSpeed * Time.deltaTime);
        if (IsGrounded && !isPlayingFootstepSound && moveDirection.magnitude > 0)
        {
            StartCoroutine(playsound());
        }
    }

    IEnumerator playsound()
    {
        isPlayingFootstepSound = true;
        yield return new WaitForSeconds(FoodstepSOundPlayDelay);
        soundEffectsManager.instance.playRandomSoundEffectsClip3D(FootStepSound, transform, Volume);
        isPlayingFootstepSound = false;
    }
}
