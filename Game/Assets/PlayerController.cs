using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public bool isGrounded;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Attack")]
    public bool isAttacking = false;
    public float attackDuration = 0.4f;

    private bool isHurt = false;
    private bool isCrouching = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (!rb) Debug.LogError("Rigidbody2D not found!");
        if (!animator) Debug.LogError("Animator not found!");
        if (!spriteRenderer) Debug.LogError("SpriteRenderer not found!");
        if (!groundCheck) Debug.LogError("GroundCheck not assigned in Inspector!");
    }

    void Update()
    {
        if (isHurt) return; // Prevent movement while hurt

        HandleMovement();
        HandleJump();
        HandleAttack();
        HandleCrouch();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void HandleMovement()
    {
        if (isAttacking || isCrouching) return; // Disable movement during attack/crouch

        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Flip character based on movement direction
        if (moveInput > 0) spriteRenderer.flipX = false;
        else if (moveInput < 0) spriteRenderer.flipX = true;

        animator.SetBool("isRunning", moveInput != 0);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("isJumping", true);
        }
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isAttacking)
        {
            isAttacking = true;
            animator.SetBool("isAttacking", true);
            Invoke(nameof(ResetAttack), attackDuration);
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            isCrouching = true;
            animator.SetBool("isCrouching", true);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            isCrouching = false;
            animator.SetBool("isCrouching", false);
        }
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isJumping", !isGrounded && rb.linearVelocity.y > 0);
        animator.SetBool("isHurt", isHurt);
    }

    public void TakeDamage()
    {
        isHurt = true;
        animator.SetBool("isHurt", true);
        Invoke(nameof(ResetHurt), 0.5f);
    }

    void ResetHurt()
    {
        isHurt = false;
        animator.SetBool("isHurt", false);
    }
}
