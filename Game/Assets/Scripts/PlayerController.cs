using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public int maxJumps = 2;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask physicalPlaneMask;
    public LayerMask spiritPlaneMask;
    public LayerMask boundaryMask;

    [Header("Plane Switching")]
    private bool inPhysicalPlane = true;

    [Header("Combat")]
    public GameObject swordHitbox;

    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    // State
    private bool isGrounded;
    private int jumpsRemaining;
    private bool isAttacking;
    private bool isCrouching;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        if (swordHitbox != null)
            swordHitbox.SetActive(false);

        jumpsRemaining = maxJumps;
        UpdatePlane();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAttack();
        HandleCrouch();
        HandlePlaneSwitching();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        LayerMask currentGroundMask = (inPhysicalPlane ? physicalPlaneMask : spiritPlaneMask) | boundaryMask;

        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, currentGroundMask);

        if (isGrounded && !wasGrounded)
        {
            jumpsRemaining = maxJumps;
        }
    }

    void HandleMovement()
    {
        if (isAttacking || isCrouching) return;

        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;
    }

    void HandleJump()
    {
        if (isAttacking || isCrouching) return;

        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsRemaining--;
            animator.SetTrigger("Jump");
        }
    }

    void HandleCrouch()
    {
        if (isAttacking) return;

        isCrouching = Input.GetKey(KeyCode.LeftControl);

        if (isCrouching)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        animator.SetBool("isCrouching", isCrouching);
    }

    void HandleAttack()
    {
        if (isAttacking) return;

        if (Input.GetMouseButtonDown(0)) // Left click
            StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true);

        if (swordHitbox != null)
            swordHitbox.SetActive(true); // Enable sword hitbox

        yield return new WaitForSeconds(0.5f); // Match animation timing

        if (swordHitbox != null)
            swordHitbox.SetActive(false); // Disable sword hitbox

        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    void HandlePlaneSwitching()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inPhysicalPlane = !inPhysicalPlane;
            UpdatePlane();
        }
    }

    void UpdatePlane()
    {
        int physicalLayer = LayerMask.NameToLayer("PhysicalPlane");
        int spiritLayer = LayerMask.NameToLayer("SpiritPlane");

        if (mainCamera)
        {
            mainCamera.cullingMask |= (1 << (inPhysicalPlane ? physicalLayer : spiritLayer));
            mainCamera.cullingMask &= ~(1 << (inPhysicalPlane ? spiritLayer : physicalLayer));
        }

        TogglePlaneColliders("PhysicalPlane", inPhysicalPlane);
        TogglePlaneColliders("SpiritPlane", !inPhysicalPlane);
    }

    void TogglePlaneColliders(string layerName, bool active)
    {
        GameObject[] objs = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in objs)
        {
            if (obj.layer == LayerMask.NameToLayer(layerName))
            {
                Collider2D col = obj.GetComponent<Collider2D>();
                if (col != null) col.enabled = active;
            }
        }
    }

    void UpdateAnimations()
    {
        animator.SetBool("isRunning", Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded && !isAttacking && !isCrouching);
        animator.SetBool("isCrouching", isCrouching);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isJumping", !isGrounded);
    }

    public bool IsAttacking()
    {
        return animator.GetBool("isAttacking");
    }
}
