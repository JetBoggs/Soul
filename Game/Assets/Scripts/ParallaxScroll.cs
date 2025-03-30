using UnityEngine;

public class ParallaxScroll : MonoBehaviour
{
    public float parallaxFactor = 0.2f;
    private Transform cam;
    private Vector3 startPos;

    void Start()
    {
        cam = Camera.main.transform;
        startPos = transform.position;
    }

    void LateUpdate()
    {
        float dist = cam.position.x * parallaxFactor;
        transform.position = new Vector3(startPos.x + dist, startPos.y, startPos.z);
    }
}
