using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public bool canHit = false;
    private bool hasHit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryHit(other);
    }

    void TryHit(Collider2D other)
    {
        if (!canHit || hasHit) return;

        if (other.CompareTag("Enemy"))
        {
            hasHit = true;
            Debug.Log("Enemy Hit!");

            // Trigger death animation
            Animator anim = other.GetComponent<Animator>();
            if (anim != null)
                anim.SetTrigger("die");

            // Disable enemy's collider
            Collider2D enemyCollider = other.GetComponent<Collider2D>();
            if (enemyCollider != null)
                enemyCollider.enabled = false;

            // Disable enemy's damage script if it has one
            MonoBehaviour damageScript = other.GetComponent<MonoBehaviour>();
            if (damageScript != null)
                damageScript.enabled = false;

            // Destroy after short delay
            Destroy(other.gameObject, 1f);
        }
    }

    private void OnDisable()
    {
        hasHit = false;
    }
}
