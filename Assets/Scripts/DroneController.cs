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
    private bool controlEnabled = true;

    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (controlEnabled)
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
            Quaternion shootRotation = Quaternion.Euler(transform.up);
            Vector3 shootPosition = transform.position - transform.right * 0.47f;
            Instantiate(laserProjectile, shootPosition, shootRotation);
        }
    }

    void ReloadLaser()
    {
        currentReloadCnt = MaxReloadCnt;
    }
}