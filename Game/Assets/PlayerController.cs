using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;

    public Transform groundCheck;
    public LayerMask groundLayer;

    private bool isFacingRight = true; // Track the player's facing direction

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Assign the SpriteRenderer that is part of your character
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();  // This grabs the SpriteRenderer from any child

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found! Make sure your character has a SpriteRenderer.");
        }
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Moving the player
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // Handle animations (running and idle)
        anim.SetBool("isRunning", moveInput != 0);

        // Flip the character (whole object) when changing direction
        if (moveInput > 0 && !isFacingRight)
        {
            Flip(); // Flip the character to face right
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip(); // Flip the character to face left
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("Jump");
        }
    }

    void FixedUpdate()
    {
        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
        anim.SetBool("isGrounded", isGrounded);
    }

    // Function to flip the player character (whole object)
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f); // Rotate the entire character to flip it
    }
}
