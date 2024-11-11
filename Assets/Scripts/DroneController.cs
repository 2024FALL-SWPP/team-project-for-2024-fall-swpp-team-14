using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DroneController : MonoBehaviour
{
    private AudioSource playerAudio;
    public AudioClip shootLaserAudio;
    float horizontalInput;
    float verticalInput;
    float elevationInput;
    public float droneSpeed = 30.0f;
    private float tiltAngle = 10f;
    public GameObject[] propellers;
    private float propellerRotateSpeed = 2000.0f;
    public GameObject laserProjectile;
    public int currentReloadCnt = 20;
    private int MaxReloadCnt = 20;
    private Transform aircraft;
    private float mouseSensitivity = 10f;
    Quaternion originalRotation;
    Rigidbody rb;
    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        aircraft = transform.Find("Aircraft1");
        originalRotation = aircraft.rotation;
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        elevationInput = Input.GetAxis("Elevation");
        Vector3 moveDirection = (Vector3.right * horizontalInput) + (Vector3.forward * verticalInput) + (Vector3.up * elevationInput);
        rb.velocity = droneSpeed*transform.TransformDirection(moveDirection);

        Quaternion tiltHorizontal = Quaternion.Euler(horizontalInput * tiltAngle, 0, 0);
        Quaternion tiltVertical = Quaternion.Euler(0, -verticalInput * tiltAngle, 0);
        aircraft.localRotation = originalRotation * tiltHorizontal * tiltVertical;

        float mouseX = Input.GetAxis("Mouse X");
        transform.rotation *= Quaternion.Euler(0, mouseX * mouseSensitivity, 0);

        UpdatePropellers();
        if (Input.GetMouseButtonDown(0))
        {
            ShootLaser();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadLaser();
        }
    }

    void UpdatePropellers()
    {
        for (int i = 0; i < propellers.Length; i++)
        {
            propellers[i].transform.Rotate(0, 0, propellerRotateSpeed * Time.deltaTime);
        }
    }

    void ShootLaser()
    {
        if (currentReloadCnt > 0)
        {
            currentReloadCnt -= 1;
            playerAudio.PlayOneShot(shootLaserAudio);
            Quaternion shootRotation = transform.rotation;
            Vector3 shootPosition = transform.position + transform.forward * 0.4f;
            Instantiate(laserProjectile, shootPosition, shootRotation);
        }
    }

    void ReloadLaser()
    {
        currentReloadCnt = MaxReloadCnt;
    }
}