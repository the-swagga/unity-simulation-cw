using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDriveSim : MonoBehaviour
{
    [SerializeField] private WheelCollider wheelFL;
    [SerializeField] private WheelCollider wheelFR;
    [SerializeField] private WheelCollider wheelBR;
    [SerializeField] private WheelCollider wheelBL;
    [SerializeField] private float speed;
    [SerializeField] private float turnAngle;
    [SerializeField] private float driveTime;
    private float driveTimer = 0.0f;
    private float turnTime = 0.0f;
    private float turnTimer = 0.0f;
    private bool forward = true;

    private void Start()
    {
        turnTime = Random.Range(0.5f, 1.5f);
        float steerAngle = Random.Range(-turnAngle, turnAngle);
        wheelFL.steerAngle = steerAngle;
        wheelFR.steerAngle = steerAngle;
    }

    private void FixedUpdate()
    {
        driveTimer += Time.fixedDeltaTime;

        if (forward)
        {
            if (driveTimer >= driveTime)
            {
                forward = false;
                driveTimer = 0.0f;
            }
        }
        else
        {
            {
                if (driveTimer >= driveTime / 2.0f)
                {
                    forward = true;
                    driveTimer = 0.0f;
                }
            }
        }

        if (forward)
        {
            wheelFL.motorTorque = speed;
            wheelFR.motorTorque = speed;
            wheelBR.motorTorque = speed;
            wheelBL.motorTorque = speed;
        } else
        {
            wheelFL.motorTorque = -speed;
            wheelFR.motorTorque = -speed;
            wheelBR.motorTorque = -speed;
            wheelBL.motorTorque = -speed;
        }

        turnTimer += Time.fixedDeltaTime;

        if (turnTimer >= turnTime)
        {
            turnTime = Random.Range(0.5f, 1.5f);
            turnTimer = 0.0f;

            float steerAngle = Random.Range(-turnAngle, turnAngle);
            wheelFL.steerAngle = steerAngle;
            wheelFR.steerAngle = steerAngle;
        }
    }
}
