using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float sensitivity = 3.0f;
    [SerializeField] private float minHeight = -30.0f;
    [SerializeField] private float maxHeight = 60.0f;

    private float yaw;
    private float pitch;
    public float GetPitch() { return pitch; }

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    private void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch += -(Input.GetAxis("Mouse Y") * sensitivity);
        pitch = Mathf.Clamp(pitch, minHeight, maxHeight);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        if (player != null)
        {
            transform.position = player.position + Vector3.up * 1.5f;
        }
    }
}
