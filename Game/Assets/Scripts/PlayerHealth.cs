using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    private Animator animator;
    private bool isInvulnerable = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isInvulnerable) return;

        currentHealth -= amount;
        Debug.Log("Player HP: " + currentHealth);

        if (animator) animator.SetTrigger("isHurt");

        if (currentHealth <= 0)
        {
            Debug.Log("Player died!");
            Destroy(gameObject); // or your death logic
        }

        StartCoroutine(InvulnerabilityFrames());
    }

    IEnumerator InvulnerabilityFrames()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(1f); // 1 second of invincibility
        isInvulnerable = false;
    }
}
