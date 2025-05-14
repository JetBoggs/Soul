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
        Debug.Log("Player took damage. HP: " + currentHealth);

        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Debug.Log("Player died!");
            // death logic here
            Destroy(gameObject);
        }

        StartCoroutine(TemporaryInvulnerability());
    }

    IEnumerator TemporaryInvulnerability()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(1f);
        isInvulnerable = false;
    }
}
