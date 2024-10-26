using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DroneController : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;
    float elevationInput;
    public float droneSpeed = 30.0f;
    private float tiltAngle = 10f;
    void Start()
    {

    }
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        elevationInput = Input.GetAxis("Elevation");

        Vector3 moveDirection = (Vector3.right * -horizontalInput) + (Vector3.forward * -verticalInput);
        transform.Translate(moveDirection * Time.deltaTime * droneSpeed, Space.World);
        transform.Translate(Vector3.up * elevationInput * Time.deltaTime * droneSpeed, Space.World);
        float tiltX = horizontalInput * tiltAngle;
        float tiltZ = verticalInput * tiltAngle;
        if (horizontalInput != 0 || verticalInput != 0)
        {
            transform.rotation = Quaternion.Euler(tiltX, -90, tiltZ);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
    }
}