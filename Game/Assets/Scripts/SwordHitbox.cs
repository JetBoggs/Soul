using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public PlayerController player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && player != null && player.IsAttacking())
        {
            Animator enemyAnim = other.GetComponent<Animator>();
            if (enemyAnim != null)
                enemyAnim.SetTrigger("die");

            Destroy(other.gameObject, 1f); // allow animation to play
        }

    }
}
