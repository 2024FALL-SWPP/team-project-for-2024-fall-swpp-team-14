using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }
    private Camera mainCamera;
    private Outline outline;
    private GameObject drone;
    private DroneController droneController;
    private bool isInteracting = false;
    Camera laptopCamera;
    private DroneUIManager droneUIManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!isInteracting)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;
            // Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red);
            if (Physics.Raycast(ray, out hit, 3f, LayerMask.GetMask("interactable")) && !isInteracting)
            {
                GameObject interactionObject = hit.collider.gameObject;
                outline = interactionObject.GetComponent<Outline>();
                outline.enabled = true;
                outline.OutlineColor = Color.red;
                if (interactionObject.tag == "Laptop")
                {
                    LaptopController labtopController = interactionObject.GetComponent<LaptopController>();
                    if (labtopController.CanInteract())
                    {
                        droneUIManager.ShowPressInteractionImage();
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            isInteracting = true;
                            labtopController.StartInteraction();
                            laptopCamera = hit.collider.gameObject.transform.Find("LaptopCamera").GetComponent<Camera>();
                            laptopCamera.enabled = true;
                            //drone.SetActive(false);
                        }

                    }
                    else
                    {
                        outline.OutlineColor = Color.green;
                    }
                }
                else if (interactionObject.tag == "Server")
                {
                    ServerController serverController = interactionObject.GetComponent<ServerController>();
                    if (serverController.CanInteract())
                    {
                        droneUIManager.ShowPressInteractionImage();
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            serverController.StartInteraction();
                        }
                    }
                    else
                    {
                        outline.OutlineColor = Color.green;
                    }
                }
            }
            else
            {
                droneUIManager.HidePressInteractionImage();
                if (outline is not null)
                {
                    outline.enabled = false;
                }
            }
        }

    }

    public void ExitInteraction(bool succeed)
    {
        isInteracting = false;
        if (laptopCamera != null)
        {
            laptopCamera.enabled = false;
        }

        drone.SetActive(true);
        droneController.EnableControl();
        if (succeed)
        {
            droneUIManager.HidePressInteractionImage();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject[] interactableObjects = GameObject.FindGameObjectsWithTag("Laptop");
        for (int i = 0; i < interactableObjects.Length; i++)
        {
            interactableObjects[i].GetComponent<Outline>().enabled = false;
        }
        interactableObjects = GameObject.FindGameObjectsWithTag("Server");
        for (int i = 0; i < interactableObjects.Length; i++)
        {
            interactableObjects[i].GetComponent<Outline>().enabled = false;
        }
        mainCamera = Camera.main;
        drone = GameObject.Find("Drone");
        droneController = drone.GetComponent<DroneController>();
        droneUIManager = drone.GetComponent<DroneUIManager>();
    }

    public Camera GetMainCamera()
    {
        return mainCamera;
    }
}