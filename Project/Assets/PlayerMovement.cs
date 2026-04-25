using UnityEngine;
using UnityEngine.SceneManagement; // Scene reload karne ke liye add kiya

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public float doubleJumpMultiplier = 0.8f; // Second jump is 80% of first jump
    
    // UI Panel references
    public GameObject gameOverPanel;
    public GameObject gameWonPanel; // Naya: Game Won Panel ka reference
    private bool isGameOver = false; // Check karne ke liye ke game over to ni hogi
    
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
        // Jab game start ho to time normal chalay
        Time.timeScale = 1f; 

        // Game start hone par panels hide kar dein (in case editor me on reh gaye hon)
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if(gameWonPanel != null) // Naya: Game Won panel ko start me hide karna
        {
            gameWonPanel.SetActive(false);
        }

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
        // Agar game over ho chuki hai to player ko move mat karne do
        if (isGameOver) return;

        // Get input
        float move = Input.GetAxisRaw("Horizontal");
        
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
            }
            else if (canDoubleJump)
            {
                // Double jump (in air)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * doubleJumpMultiplier);
                canDoubleJump = false;
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
            if (rb.velocity.x < 0) rb.velocity = new Vector2(0, rb.velocity.y);
            clamped = true;
        }
        else if (playerPos.x > maxX)
        {
            playerPos.x = maxX;
            if (rb.velocity.x > 0) rb.velocity = new Vector2(0, rb.velocity.y);
            clamped = true;
        }
        
        // Clamp Y position using fixed bounds
        float minY = bottomBound + playerHeight;
        float maxY = topBound - playerHeight;
        
        if (playerPos.y < minY)
        {
            playerPos.y = minY;
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
            if (rb.velocity.y > 0) rb.velocity = new Vector2(rb.velocity.x, 0);
            clamped = true;
        }
        
        // Apply clamped position
        if (clamped)
        {
            transform.position = playerPos;
        }
    }
    
    // Regular Collisions (Zameen aur Spikes)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            canDoubleJump = false; 
        }
        // Agar spike se takraye
        else if (collision.gameObject.CompareTag("Spike"))
        {
            GameOver();
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // NAYA: Triggers ke liye (Jab player Key ke andar se guzrega)
    void OnTriggerEnter2D(Collider2D other)
    {
        // Agar touch hone wali cheez ka tag "Key" hai
        if (other.gameObject.CompareTag("KeyGold"))
        {
            Destroy(other.gameObject);
            GameWon();
        }
    }

    // GAME OVER LOGIC
    void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over!");
        
        // UI Panel show karein
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Game ko pause kar dein
        Time.timeScale = 0f; 
    }

    // NAYA: GAME WON LOGIC
    void GameWon()
    {
        isGameOver = true;
        Debug.Log("Level Cleared!");
        
        // Win UI Panel show karein
        if(gameWonPanel != null)
        {
            gameWonPanel.SetActive(true);
        }

        // Game ko pause kar dein
        Time.timeScale = 0f; 
    }

    // BUTTON CLICK LOGIC (Dono panels ke liye yahi use hoga)
    public void RestartGame()
    {
        // Time dobara normal kar dein aur scene reload kar lein
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}