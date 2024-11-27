using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class DroneController : MonoBehaviour
{
    public int droneHp = 100;

    private AudioSource playerAudio;
    public AudioClip shootLaserAudio;
    public AudioClip AlertAudio;
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
    private MainMapManager mainMapManager;
    private bool isAlertPlayed = false;
    private DroneUIManager droneUIManager;
    public AudioClip droneDeathAudio;
    public AudioClip droneDamageAudio;
    public ParticleSystem droneDeathParticle;

    public enum DroneGameState { InGame, GameOver, MapClear };

    public DroneGameState droneGameState;
    private float lastDamagedTimeByLaserObstacle = 0;
    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        aircraft = transform.Find("Aircraft1");
        originalRotation = aircraft.rotation;
        rb = GetComponent<Rigidbody>();
        droneCamera = transform.Find("Main Camera");

        if (GameObject.Find("MainMapManager") != null)
        {
            mainMapManager = GameObject.Find("MainMapManager").GetComponent<MainMapManager>();
        }
        else
        {
            mainMapManager = null;
        }
        
        isAlertPlayed = false;
        droneUIManager = GetComponent<DroneUIManager>();
        droneGameState = DroneGameState.InGame;
        droneUIManager.ShowInGameScreen();
    }
    void Update()
    {
        if (controlEnabled && droneGameState == DroneGameState.InGame)
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
            if (mainMapManager != null && mainMapManager.isServerActivated && !isAlertPlayed)
            {
                playerAudio.PlayOneShot(AlertAudio);
                isAlertPlayed = true;
            }
        }
        UpdatePropellers();

    }
    public void EnableControl()
    {
        controlEnabled = true;
    }

    void LateUpdate()
    {
        if (controlEnabled && droneGameState == DroneGameState.InGame)
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
        if (droneGameState == DroneGameState.InGame)
        {
            if (other.CompareTag("Laser"))
            {
                playerAudio.PlayOneShot(droneDamageAudio);
                DroneGetDamaged(10);
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (droneGameState == DroneGameState.InGame)
        {
            if (other.CompareTag("LaserObstacle"))
            {
                playerAudio.PlayOneShot(droneDamageAudio);
                if (Time.time - lastDamagedTimeByLaserObstacle > 0.5f)
                {
                    DroneGetDamaged(10);
                    lastDamagedTimeByLaserObstacle = Time.time;
                }
            }
        }
    }

    void DroneGetDamaged(int damage)
    {
        droneHp -= damage;

        if (droneHp <= 0)
        {

            GameOver();
        }
    }

    public void GameOver()
    {
        GameObject alert_red = GameObject.Find("Alert_Red");
        if (alert_red != null)
        {
            alert_red.SetActive(false);
        }
        
        playerAudio.PlayOneShot(droneDeathAudio);
        droneDeathParticle.Play();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(Vector3.up * 3.0f, ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f)), ForceMode.Impulse);
        droneGameState = DroneGameState.GameOver;
        droneUIManager.ShowGameOverScreen();
    }

    public void MapClear()
    {
        droneGameState = DroneGameState.MapClear;
        droneUIManager.ShowMapClearScreen();
    }
}