using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public int maxJumps = 2;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask physicalPlaneMask;
    public LayerMask spiritPlaneMask;
    public LayerMask boundaryMask;

    [Header("Plane Switching")]
    public bool inPhysicalPlane = true;

    [Header("Combat")]
    public GameObject swordHitbox;

    [Header("Health & Respawn")]
    public int maxHealth = 5;
    public float hurtCooldown = 1f;
    public Transform spawnPoint;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip musicTheme;
    public AudioClip jumpClip;
    public AudioClip walkClip;
    public AudioClip hitClip;
    public AudioClip hurtClip;
    public AudioClip boomClip;

    // State
    int currentHealth;
    int jumpsRemaining;
    bool isGrounded;
    bool isHurt;
    bool isAttacking;
    bool isCrouching;

    // Components
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        currentHealth = maxHealth;
        jumpsRemaining = maxJumps;
        UpdatePlane();

        if (swordHitbox != null)
            swordHitbox.SetActive(false);

        if (musicSource && musicTheme)
        {
            musicSource.clip = musicTheme;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    void Update()
    {
        HandleMovement();
        FlipSwordHitbox();
        HandleJump();
        HandleAttack();
        HandleCrouch();
        HandlePlaneSwitching();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        LayerMask groundMask = (inPhysicalPlane ? physicalPlaneMask : spiritPlaneMask) | boundaryMask;
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
        if (isGrounded && !wasGrounded)
            jumpsRemaining = maxJumps;
    }

    void HandleMovement()
    {
        if (isAttacking || isCrouching) return;

        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput != 0)
        {
            spriteRenderer.flipX = moveInput < 0;
            if (walkClip && !sfxSource.isPlaying)
                sfxSource.PlayOneShot(walkClip);
        }
    }

    void HandleJump()
    {
        if (isAttacking || isCrouching) return;

        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsRemaining--;
            animator.SetTrigger("Jump");
            if (jumpClip)
                sfxSource.PlayOneShot(jumpClip);
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

        if (Input.GetMouseButtonDown(0))
            StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true);

        if (hitClip)
            sfxSource.PlayOneShot(hitClip);

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    public void EnableHit()
    {
        if (swordHitbox != null)
        {
            swordHitbox.SetActive(true);
            var hitScript = swordHitbox.GetComponent<SwordHitbox>();
            if (hitScript != null) hitScript.canHit = true;
        }
    }

    public void DisableHit()
    {
        if (swordHitbox != null)
        {
            var hitScript = swordHitbox.GetComponent<SwordHitbox>();
            if (hitScript != null) hitScript.canHit = false;
            swordHitbox.SetActive(false);
        }
    }

    void FlipSwordHitbox()
    {
        if (swordHitbox == null) return;

        float direction = spriteRenderer.flipX ? -1f : 1f;
        Vector3 pos = swordHitbox.transform.localPosition;
        pos.x = Mathf.Abs(pos.x) * direction;
        swordHitbox.transform.localPosition = pos;
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

    public void TakeDamage(int damage)
    {
        if (isHurt) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            StartCoroutine(DieAndRespawn());
            return;
        }

        StartCoroutine(HurtRoutine());
    }

    IEnumerator HurtRoutine()
    {
        isHurt = true;
        animator.SetTrigger("Hurt");
        if (hurtClip)
            sfxSource.PlayOneShot(hurtClip);

        yield return new WaitForSeconds(hurtCooldown);
        isHurt = false;
    }

    IEnumerator DieAndRespawn()
    {
        animator.SetTrigger("Hurt");
        if (boomClip)
            sfxSource.PlayOneShot(boomClip);

        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(1f);

        transform.position = spawnPoint.position;
        currentHealth = maxHealth;
        rb.isKinematic = false;
        GetComponent<Collider2D>().enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard"))
        {
            StartCoroutine(DieAndRespawn());
        }
        else if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }
}
