using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damageAmount = 1;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth ph = other.gameObject.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damageAmount);
        }
    }
}
