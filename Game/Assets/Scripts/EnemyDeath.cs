using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public AudioClip deathSound;
    public GameObject deathEffect; // Optional visual effect prefab
    private AudioSource audioSource;
    private Animator animator;
    private Collider2D col;
    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // Play death animation
        animator.SetTrigger("Die");

        // Disable collider and physics
        if (col != null) col.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Play death sound
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Optional: trigger VFX
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Destroy after animation/sound
        Destroy(gameObject, 2f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerSword"))
        {
            Die();
        }
    }
}
