using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform leftPoint;
    public Transform rightPoint;
    public float speed = 2f;
    public bool movingRight = true;

    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Auto-assign patrol points from parent
        if (leftPoint == null)
            leftPoint = transform.Find("LeftPoint");
        if (rightPoint == null)
            rightPoint = transform.Find("RightPoint");
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        if (movingRight)
        {
            rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
            if (transform.position.x > rightPoint.position.x)
                Flip();
        }
        else
        {
            rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);
            if (transform.position.x < leftPoint.position.x)
                Flip();
        }

        if (animator != null)
            animator.SetBool("isRunning", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
