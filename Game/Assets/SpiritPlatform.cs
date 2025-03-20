using UnityEngine;

public class SpiritPlatform : MonoBehaviour
{
    private Collider2D platformCollider;
    private SpriteRenderer spriteRenderer;
    private bool isInSpiritPlane = false;

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isInSpiritPlane = !isInSpiritPlane;
            platformCollider.enabled = isInSpiritPlane;
            spriteRenderer.enabled = isInSpiritPlane;
        }
    }
}
