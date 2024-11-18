using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DroneController : MonoBehaviour
{
    private int droneHp = 100;

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
    private Transform droneCamera;
    private bool canShoot = true;
    private bool controlEnabled = true;

    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        aircraft = transform.Find("Aircraft1");
        originalRotation = aircraft.rotation;
        rb = GetComponent<Rigidbody>();
        droneCamera = transform.Find("Main Camera");
    }
    void Update()
    {
        if (controlEnabled)
        {

            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            elevationInput = Input.GetAxis("Elevation");
            Vector3 moveDirection = (Vector3.right * horizontalInput) + (Vector3.forward * verticalInput) + (Vector3.up * elevationInput);
            rb.velocity = droneSpeed * transform.TransformDirection(moveDirection);

            Quaternion tiltHorizontal = Quaternion.Euler(horizontalInput * tiltAngle, 0, 0);
            Quaternion tiltVertical = Quaternion.Euler(0, -verticalInput * tiltAngle, 0);
            aircraft.localRotation = originalRotation * tiltHorizontal * tiltVertical;

            float mouseX = Input.GetAxis("Mouse X");
            transform.rotation *= Quaternion.Euler(0, mouseX * mouseSensitivity, 0);

            if (Input.GetMouseButtonDown(0))
            {
                ShootLaser();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReloadLaser();
            }
        }
        UpdatePropellers();

    }
    public void EnableControl()
    {
        controlEnabled = true;
    }

    public void DisableControl()
    {
        controlEnabled = false;
    }
    void LateUpdate()
    {
        if (controlEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                droneCamera.localPosition = new Vector3(0, 16, -50);
                droneCamera.localRotation = Quaternion.Euler(0, 0, 0);
                canShoot = true;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                droneCamera.localPosition = new Vector3(0, -12, -50);
                droneCamera.localRotation = Quaternion.Euler(-45, 0, 0);
                canShoot = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                droneCamera.localPosition = new Vector3(0, 16, -50);
                droneCamera.localRotation = Quaternion.Euler(45, 0, 0);
                canShoot = false;
            }
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
        if (currentReloadCnt > 0 && canShoot)
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Laser"))
        { //if hit by enemy's laser, decrease HP
            droneHp -= 10;
            Debug.Log("drone hp decrease");
        }
    }
}