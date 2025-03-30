using UnityEngine;

public class ParallaxRepeater : MonoBehaviour
{
    public float spriteWidth = 16f; // width of one tile in world units
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        float camDist = cam.position.x - transform.position.x;

        if (Mathf.Abs(camDist) >= spriteWidth)
        {
            float offset = camDist % spriteWidth;
            transform.position = new Vector3(cam.position.x + offset, transform.position.y, transform.position.z);
        }
    }
}
