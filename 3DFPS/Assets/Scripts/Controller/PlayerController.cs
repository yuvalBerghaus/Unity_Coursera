using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the movement of the player with given input from the input manager
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The speed at which the player moves")]
    public float moveSpeed = 2f;
    [Tooltip("The speed at which the player looks")]
    public float lookspeed = 60f;
    [Tooltip("The power of which the player jumps")]
    public float jumpPower = 8f;
    [Tooltip("The gravity of the world")]
    public float gravity = 9.81f;

    [Header("Jump Timing")]
    public float jumpTimeLeniency = 0.1f;
    float timeToStopBeingLenient = 0;
    [Header("Required References")]
    public Shooter playerShooter;
    bool doubleJumpAvailable = false;

    [Tooltip("The player shooter script that fires projectiles")]
    // The character controller component on the player
    private CharacterController controller;
    private InputManager inputManager;
    /// <summary>
    /// Description:
    /// Standard Unity function called once before the first Update call
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Start()
    {
        SetUpCharacterController();
        SetUpInputManager();
        ProcessRotation();
    }



    private void SetUpCharacterController()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("The player controller script doesnt have a character controller on the same game object!");
        }
    }

    void SetUpInputManager()
    {
        inputManager = InputManager.instance;
    }
    /// <summary>
    /// Description:
    /// Standard Unity function called once every frame
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Update()
    {
        ProcessMovement();
        ProcessRotation();
    }
    Vector3 moveDirection;
    void ProcessMovement()
    {
        // Get the input from the input manager
        float leftRightInput = inputManager.horizontalMoveAxis;
        float forwardBackwardInput = inputManager.verticalMoveAxis;
        bool jumpPressed = inputManager.jumpPressed;
        // Handle the control of the player while it is on the ground
        if (controller.isGrounded)
        {
            doubleJumpAvailable = true;
            timeToStopBeingLenient = Time.time + jumpTimeLeniency;
            // Set the movement direction to be the recieved input, set y to 0 since we are on the ground
            moveDirection = new Vector3(leftRightInput, 0, forwardBackwardInput);
            // Set the move direction in relation to the transform
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection = moveDirection * moveSpeed;
            if (jumpPressed)
            {
                moveDirection.y = jumpPower;
            }
        }
        else
        {
            moveDirection = new Vector3(leftRightInput * moveSpeed, moveDirection.y, forwardBackwardInput * moveSpeed);
            moveDirection = transform.TransformDirection(moveDirection);
            
            if (jumpPressed && Time.time < timeToStopBeingLenient)
            {
                moveDirection.y = jumpPower;
            }
            else if(jumpPressed && doubleJumpAvailable)
            {
                moveDirection.y = jumpPower;
                doubleJumpAvailable = false;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;

        if (controller.isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = -0.3f;
        }

        controller.Move(moveDirection * Time.deltaTime);
    }

    void ProcessRotation()
    {
        float horizontalLookInput = inputManager.horizontalLookAxis;
        Vector3 playerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(playerRotation.x, playerRotation.y + horizontalLookInput * lookspeed * Time.deltaTime, playerRotation.z));
    }
}
