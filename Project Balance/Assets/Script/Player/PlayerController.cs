using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private enum PlayerCurrentState
    {
        Walking,
        Running,
    }

    [SerializeField] private PlayerCurrentState playerCurrentState;
    private Vector3 moveDirection;

    [Header("Reference")]
    [SerializeField] private Transform PlayerCamera;
    private CharacterController characterController;

    [Header("Movement")]
    [SerializeField, Range(1, 20)] private float fWalkSpeed = 20f;
    [SerializeField, Range(1, 20)] private float fRunSpeed = 30f;
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

    [Header("KeyBinding")]
    [SerializeField] private KeyCode RunKey = KeyCode.LeftShift;


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

        
        Move();
        

    }

    private void PlayerInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection = moveDirection.normalized;
    }

    private void UpdatePlayerState()
    {
        if (Input.GetKey(RunKey) && moveDirection != Vector3.zero)
        {
            playerCurrentState = PlayerCurrentState.Running;
        }
        else
        {
            playerCurrentState = PlayerCurrentState.Walking;
        }

        switch (playerCurrentState)
        {
            case PlayerCurrentState.Running:
                fMoveSpeed = fRunSpeed;
                break;

            case PlayerCurrentState.Walking:
                fMoveSpeed = fWalkSpeed;
                break;
        }
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
    }

   
}
