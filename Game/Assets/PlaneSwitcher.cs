using UnityEngine;

public class PlaneSwitcher : MonoBehaviour
{
    private bool isInSpiritPlane = false;
    private SpriteRenderer spriteRenderer;
    public Color physicalColor = Color.white;
    public Color spiritColor = Color.cyan;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isInSpiritPlane = !isInSpiritPlane;
            spriteRenderer.color = isInSpiritPlane ? spiritColor : physicalColor;
        }
    }
}
