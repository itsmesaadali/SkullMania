using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public float doubleJumpMultiplier = 0.8f; // Second jump is 80% of first jump
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canDoubleJump;
    private Camera mainCamera;
    private float playerWidth;
    private float playerHeight;
    
    // Fixed camera bounds (you can adjust these values)
    private float leftBound = -11f;
    private float rightBound = 11.20f;
    private float bottomBound = -3f;
    private float topBound = 3.5f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        
        // Get player sprite bounds for accurate collision
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            playerWidth = spriteRenderer.bounds.extents.x;
            playerHeight = spriteRenderer.bounds.extents.y;
        }
        else
        {
            // Fallback to collider bounds if no sprite renderer
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
        
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D on Player!");
        }
    }
    
    void Update()
    {
        // Get input
        float move = Input.GetAxisRaw("Horizontal");
        
        // Log what's happening
        Debug.Log($"Move Input: {move}, Velocity X: {rb.velocity.x}, Is Grounded: {isGrounded}, Can Double Jump: {canDoubleJump}");
        
        // Move the player
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
        
        // Jump handling
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                // First jump (from ground)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canDoubleJump = true;
                Debug.Log("First Jump!");
            }
            else if (canDoubleJump)
            {
                // Double jump (in air)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * doubleJumpMultiplier);
                canDoubleJump = false;
                Debug.Log("Double Jump!");
            }
        }
        
        // Keep player inside camera bounds
        KeepInCameraBounds();
    }
    
    void KeepInCameraBounds()
    {
        // Get current player position
        Vector3 playerPos = transform.position;
        bool clamped = false;
        
        // Clamp X position using fixed bounds
        float minX = leftBound + playerWidth;
        float maxX = rightBound - playerWidth;
        
        if (playerPos.x < minX)
        {
            playerPos.x = minX;
            // Stop horizontal movement when hitting left wall
            if (rb.velocity.x < 0) rb.velocity = new Vector2(0, rb.velocity.y);
            clamped = true;
        }
        else if (playerPos.x > maxX)
        {
            playerPos.x = maxX;
            // Stop horizontal movement when hitting right wall
            if (rb.velocity.x > 0) rb.velocity = new Vector2(0, rb.velocity.y);
            clamped = true;
        }
        
        // Clamp Y position using fixed bounds
        float minY = bottomBound + playerHeight;
        float maxY = topBound - playerHeight;
        
        if (playerPos.y < minY)
        {
            playerPos.y = minY;
            // Reset double jump when hitting bottom
            if (rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                isGrounded = true;
                canDoubleJump = false;
            }
            clamped = true;
        }
        else if (playerPos.y > maxY)
        {
            playerPos.y = maxY;
            // Stop jumping when hitting top
            if (rb.velocity.y > 0) rb.velocity = new Vector2(rb.velocity.x, 0);
            clamped = true;
        }
        
        // Apply clamped position
        if (clamped)
        {
            transform.position = playerPos;
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Hit: {collision.gameObject.name}");
        
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            canDoubleJump = false; // Reset double jump when landing
            Debug.Log("Grounded! Double jump reset.");
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Left ground!");
        }
    }
    
    // Optional: Visualize bounds in Scene view for debugging
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 bottomLeft = new Vector3(leftBound, bottomBound, 0);
        Vector3 topRight = new Vector3(rightBound, topBound, 0);
        Vector3 size = topRight - bottomLeft;
        Gizmos.DrawWireCube(bottomLeft + size / 2, size);
    }
}