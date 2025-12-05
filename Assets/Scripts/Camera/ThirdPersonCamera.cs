using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float sensitivity = 3.0f;
    [SerializeField] private float minHeight = -30.0f;
    [SerializeField] private float maxHeight = 60.0f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float camSpeed;

    private float yaw;
    private float pitch;
    public float GetYaw() { return yaw; }
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
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, minHeight, maxHeight);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0.0f);

        Vector3 targetPosition = player.transform.position + (transform.rotation * offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, camSpeed * Time.deltaTime);
    }
}
