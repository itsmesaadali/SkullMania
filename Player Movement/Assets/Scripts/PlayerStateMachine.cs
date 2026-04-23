using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 7f;
    public float doubleJumpMultiplier = 0.8f;

    [Header("Camera Bounds")]
    public float leftBound = -11f;
    public float rightBound = 11.20f;
    public float bottomBound = -3f;
    public float topBound = 3.5f;

    // Components
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    // State Machine
    private PlayerBaseState currentState;
    private PlayerStateFactory states;

    // Public properties for states
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool canDoubleJump;
    [HideInInspector] public float moveInput;
    [HideInInspector] public float playerWidth;
    [HideInInspector] public float playerHeight;

    void Start()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Calculate player bounds
        CalculatePlayerBounds();

        // Initialize states
        states = new PlayerStateFactory(this);

        // Start with idle state
        currentState = states.Idle();
        currentState.Enter();

        if (rb == null)
            Debug.LogError("No Rigidbody2D on Player!");
    }

    void CalculatePlayerBounds()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            playerWidth = spriteRenderer.bounds.extents.x;
            playerHeight = spriteRenderer.bounds.extents.y;
        }
        else
        {
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                playerWidth = collider.bounds.extents.x;
                playerHeight = collider.bounds.extents.y;
            }
            else
            {
                playerWidth = 0.5f;
                playerHeight = 0.5f;
            }
        }
    }

    void Update()
    {
        // Get input
        moveInput = Input.GetAxisRaw("Horizontal");

        // Update current state
        currentState.Update();

        // Keep player in bounds
        KeepInCameraBounds();
    }

    public void ChangeState(PlayerBaseState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    void KeepInCameraBounds()
    {
        Vector3 playerPos = transform.position;
        Vector3 newPos = playerPos;
        bool clamped = false;

        // X bounds
        float minX = leftBound + playerWidth;
        float maxX = rightBound - playerWidth;

        if (playerPos.x < minX)
        {
            newPos.x = minX;
            if (rb.velocity.x < 0) rb.velocity = new Vector2(0, rb.velocity.y);
            clamped = true;
        }
        else if (playerPos.x > maxX)
        {
            newPos.x = maxX;
            if (rb.velocity.x > 0) rb.velocity = new Vector2(0, rb.velocity.y);
            clamped = true;
        }

        // Y bounds - MODIFIED: Don't force grounded at top
        float minY = bottomBound + playerHeight;
        float maxY = topBound - playerHeight;

        if (playerPos.y < minY)
        {
            newPos.y = minY;
            if (rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            clamped = true;
        }
        else if (playerPos.y > maxY)
        {
            newPos.y = maxY;
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            clamped = true;
        }

        if (clamped)
            transform.position = newPos;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            canDoubleJump = false;

            Debug.Log("Touched ground! Current state: " + currentState.GetType().Name);

            // Only change state if we're in an air state
            if (currentState is JumpingState || currentState is FallingState || currentState is DoubleJumpState)
            {
                if (Mathf.Abs(moveInput) > 0)
                {
                    Debug.Log("Switching to Walking after landing");
                    ChangeState(states.Walking());
                }
                else
                {
                    Debug.Log("Switching to Idle after landing");
                    ChangeState(states.Idle());
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 bottomLeft = new Vector3(leftBound, bottomBound, 0);
        Vector3 topRight = new Vector3(rightBound, topBound, 0);
        Vector3 size = topRight - bottomLeft;
        Gizmos.DrawWireCube(bottomLeft + size / 2, size);
    }
}