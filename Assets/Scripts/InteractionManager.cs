using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractionManager : MonoBehaviour
{
    private Outline outline;
    private GameObject drone;
    private DroneController droneController;
    private bool isInteracting = false;
    Camera laptopCamera;
    private DroneUIManager droneUIManager;
    private RiveAnimationManager riveAnimationManager;
    private Renderer droneRenderer;


    private void Start()
    {
        if (GameObject.Find("RiveAnimationManager") != null)
        {
            riveAnimationManager = GameObject.Find("RiveAnimationManager").GetComponent<RiveAnimationManager>();
        }
        else
        {
            riveAnimationManager = null;
        }
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
        drone = GameObject.Find("Drone");
        droneController = drone.GetComponent<DroneController>();
        droneUIManager = drone.GetComponent<DroneUIManager>();
        droneRenderer = GameObject.Find("Aircraft1").GetComponent<Renderer>();
        droneRenderer.enabled = true;
    }

    void Update()
    {
        if (!isInteracting)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;
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
                            // drone.SetActive(false);
                            droneRenderer.enabled = false;
                            droneController.DisableControl();
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
                            if (riveAnimationManager != null)
                            {
                                riveAnimationManager.isMainMapMissionCleared[2] = true;
                            }
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

        // drone.SetActive(true);
        droneRenderer.enabled = true;
        droneController.EnableControl();
        if (succeed)
        {
            droneUIManager.HidePressInteractionImage();
            if (riveAnimationManager != null)
            {
                riveAnimationManager.isMainMapMissionCleared[1] = true;
            }
        }
    }

}