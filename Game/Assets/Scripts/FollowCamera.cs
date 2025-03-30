using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(cam.position.x, cam.position.y, transform.position.z);
    }
}
