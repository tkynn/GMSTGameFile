using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using Ink.Runtime;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    private PlayerControls playerControls;
    private Rigidbody2D rb;

    [SerializeField] private float speed, jumpSpeed, sprintSpeed, walkSpeed, jumpSpeed2;
    [SerializeField] private LayerMask ground;
    public GameObject pausemenu; //new//
    public GameObject mapmenu; //new//
    public GameObject controlmenu; //new//
    public GameObject inventorymenu; //new//

    private Collider2D col;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 input;

    private float jumpCooldownTime = 1f;  // Time in seconds for cooldown
    private float lastJumpTime = -1f;     // Time of the last jump

    // Coyote Time Variables
    private float coyoteTime = 0.3f; // Grace period after leaving the ground
    private float coyoteTimeCounter; // Tracks how much coyote time is left


    [HideInInspector] public bool ledgeDetected; //checks for ledge

    [Header("Ledge Info")]
    [SerializeField] private Vector2 offset1;
    [SerializeField] private Vector2 offset2;
    [SerializeField] private Vector2 offset1Left;
    [SerializeField] private Vector2 offset2Left;

    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;

    private bool canGrabLedge = true;
    private bool canClimb;

    private Transform ledgeCheck;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Find the LedgeCheck child object
        ledgeCheck = transform.Find("LedgeCheck");
        if (ledgeCheck == null)
        {
            Debug.LogError("LedgeCheck child GameObject not found!");
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Start()
    {
        playerControls.Travel.Jump.performed += _ => Jump();
        playerControls.Travel.Sprint.performed += _ => Sprint(); //new//
        playerControls.Travel.Sprint.canceled += _ => SprintEnd(); //new//
        rb = GetComponent<Rigidbody2D>(); //new//
        transform.position = Vector3.zero;
    }

    private void Jump()
    {
        if (PauseManager.paused)
            return;

        // Get current time
        float currentTime = Time.time;

        // Check if the player is grounded or within coyote time
        bool canJump = (IsGrounded() || coyoteTimeCounter > 0) &&
                       !DialogueManager.GetInstance().dialogueIsPlaying &&
                       !pausemenu.activeSelf && !mapmenu.activeSelf &&
                       !controlmenu.activeSelf && !inventorymenu.activeSelf &&
                       currentTime - lastJumpTime >= jumpCooldownTime;

        if (canJump)
        {
            // Perform the jump
            rb.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
            animator.SetTrigger("Jump");
            Debug.Log("jumped");
            // Reset coyote time counter
            coyoteTimeCounter = 0;

            // Update last jump time to current time
            lastJumpTime = currentTime;

            // Face the direction the player was previously moving
            float movementInput = playerControls.Travel.Move.ReadValue<float>();
            if (movementInput != 0) // Only update direction if there was movement input
            {
                spriteRenderer.flipX = (movementInput == -1); // Flip sprite if moving left
            }
        }
        else if (currentTime - lastJumpTime < jumpCooldownTime)
        {
            // Prevent jumping if within cooldown time
            Debug.Log("Jump is on cooldown.");
        }
    }


    private IEnumerator ClimbLedge()
    {
        // Get the player's collider and the ground collider
        Collider2D playerCollider = GetComponent<Collider2D>();
        Collider2D groundCollider = GameObject.FindWithTag("ClimbLedge").GetComponent<Collider2D>(); // Adjust tag as needed

        if (playerCollider != null && groundCollider != null)
        {
            // Ignore collisions between the player and the ground
            Physics2D.IgnoreCollision(playerCollider, groundCollider, true);
        }

        float climbDuration = .51f; // Adjust this to match the duration of the climb animation
        float elapsedTime = 0f;

        Vector2 startPosition = transform.position;

        while (elapsedTime < climbDuration)
        {
            // Smoothly move the player to the climbOverPosition
            transform.position = Vector2.Lerp(startPosition, climbOverPosition, elapsedTime / climbDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the player is at the final position
        transform.position = climbOverPosition;

        if (playerCollider != null && groundCollider != null)
        {
            // Re-enable collisions between the player and the ground
            Physics2D.IgnoreCollision(playerCollider, groundCollider, false);
        }

        // Reset variables after climbing
        canClimb = false;
        ledgeDetected = false;
        canGrabLedge = true;

        // Reset the animator parameter
        animator.SetBool("canClimb", false);
    }

    private void Sprint() //new//
    {
        if (PauseManager.paused)
            return;

        float movementInput = playerControls.Travel.Move.ReadValue<float>();
        if (movementInput != 0 && IsGrounded())
        {
            speed = sprintSpeed;
            animator.speed = 1.5f; //speed change animation//
        }
        else
        {
            speed = walkSpeed;
            animator.speed = 1f;
            //return to normal//
        }
    }

    private void SprintEnd() //new//
    {
        if (PauseManager.paused)
            return;

        float movementInput = playerControls.Travel.Move.ReadValue<float>();
        if (movementInput == 0 && IsGrounded())
        {
            speed = walkSpeed; //speed change//
            animator.speed = 1f;
        }
        else
        {
            speed = walkSpeed;
            animator.speed = 1f; //return to normal//
        }
    }

    public bool IsGrounded()
    {
        Vector2 feetPos = transform.position;
        feetPos.y -= col.bounds.extents.y;
        return Physics2D.OverlapCircle(feetPos, 1f, ground);
    }


    private void Update()
    {
        if (PauseManager.paused)
            return;

        CheckForLedge(); //checks for ledge

        // Check for Interact2 input to initiate climb (ledge grab)
        if (playerControls.Travel.Interact2.triggered && ledgeDetected && canGrabLedge)
        {
            canClimb = true;
        }

        if (canClimb)
        {
            animator.SetBool("canClimb", true);
            StartCoroutine(ClimbLedge());
        }
        else
        {
            animator.SetBool("canClimb", false);
        }

        // Update coyote time counter
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime; // Reset coyote time when grounded
            Debug.Log("Is Grounded");
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Decrease coyote time when not grounded
            Debug.Log("Is Not Grounded");
        }

        Debug.Log(ledgeDetected); //checks if ledge is detected

        // Animation

        if (IsGrounded() && animator.GetBool("Jump"))
        {
            animator.SetBool("Jump", false);
        }

        if (playerControls.Travel.Jump.triggered)
        {
            Debug.Log("Jump");
            animator.SetBool("Jump", true);
        }
        if (playerControls.Travel.Interact1.triggered)
        {
            animator.SetBool("Walk", false);
            Debug.Log("Interact1");
        }
        if (playerControls.Travel.Interact2.triggered)
        {
            Debug.Log("Interact2");
        }
        if (playerControls.Travel.Interact3.triggered)
        {
            Debug.Log("Interact3");
        }
        if (playerControls.Travel.Interact4.triggered)
        {
            Debug.Log("Interact4");
        }
        if (playerControls.Travel.Sprint.triggered)
        {
            Debug.Log("Sprinting");
        }
        if (playerControls.Travel.Interact5.triggered)
        {
            Debug.Log("Interact5");
        }

        if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }
        if (rb.constraints != RigidbodyConstraints2D.FreezeAll)
        {
            Move();
        }
    }

    private void CheckForLedge()
    {
        // Get the LedgeCheck child GameObject
        Transform ledgeCheck = transform.Find("LedgeCheck");

        if (ledgeCheck == null)
        {
            Debug.LogError("LedgeCheck child GameObject not found!");
            return;
        }

        // Flip the X position of LedgeCheck based on the player's facing direction
        if (!spriteRenderer.flipX) // Player is facing left
        {
            ledgeCheck.localPosition = new Vector3(-Mathf.Abs(ledgeCheck.localPosition.x), ledgeCheck.localPosition.y, ledgeCheck.localPosition.z);
        }
        else // Player is facing right
        {
            ledgeCheck.localPosition = new Vector3(Mathf.Abs(ledgeCheck.localPosition.x), ledgeCheck.localPosition.y, ledgeCheck.localPosition.z);
        }

        // Check for ledge detection
        if (playerControls.Travel.Interact2.triggered && ledgeDetected && canGrabLedge && spriteRenderer.flipX) //interact2 check
        {
            canGrabLedge = false;

            Vector2 ledgePosition = ledgeCheck.position;

            climbBegunPosition = ledgePosition + offset1;
            climbOverPosition = ledgePosition + offset2;

            canClimb = true;

        }
        else if (playerControls.Travel.Interact2.triggered && ledgeDetected && canGrabLedge && !spriteRenderer.flipX) //interact 2 check
        {
            canGrabLedge = false;

            Vector2 ledgePosition = ledgeCheck.position;

            climbBegunPosition = ledgePosition + offset1Left;
            climbOverPosition = ledgePosition + offset2Left;

            canClimb = true;
        }

        if (canClimb)
        {
            transform.position = climbBegunPosition;
        }
    }

    private void Move()
    {
        float movementInput = playerControls.Travel.Move.ReadValue<float>();

        Vector3 currentPosition = transform.position;
        currentPosition.x += movementInput * speed * Time.deltaTime;
        transform.position = currentPosition;

        // Animation
        if (movementInput != 0) animator.SetBool("Walk", true);
        else animator.SetBool("Walk", false);

        // Sprite Flip
        if (movementInput == -1)
            spriteRenderer.flipX = false;
        else if (movementInput == 1)
            spriteRenderer.flipX = true;
    }

    public void LoadData(GameData data)
    {
        this.transform.position = data.playerPosition;
    }

    public void SaveData(GameData data)
    {
        data.playerPosition = this.transform.position;
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
    public bool IsInteract1Triggered()
    {
        return playerControls.Travel.Interact1.triggered;
    } //for the teleporttrigger script
}