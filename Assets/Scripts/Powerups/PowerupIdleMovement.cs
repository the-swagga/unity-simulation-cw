using System;
using System.Collections;
using UnityEngine;

public class PowerupIdleMovement : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 45, 0);
    [SerializeField] private float bobSpeed = 3.0f;
    [SerializeField] private float bobDistance = 0.1f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }
    
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);

        float newPos = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobDistance;
        transform.position = new Vector3(transform.position.x, newPos, startPos.z);
    }
}
