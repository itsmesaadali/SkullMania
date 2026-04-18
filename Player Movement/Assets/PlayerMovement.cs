using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
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
        Debug.Log($"Move Input: {move}, Velocity X: {rb.velocity.x}");
        
        // Move the player
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
        
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            Debug.Log("Jump pressed!");
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Hit: {collision.gameObject.name}");
        
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Grounded!");
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
}