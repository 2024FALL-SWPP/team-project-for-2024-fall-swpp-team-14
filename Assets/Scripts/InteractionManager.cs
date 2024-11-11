using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }
    private Camera mainCamera;
    private float interactionRange = 1.0f;
    // public LayerMask interactionLayer;
    private GameObject highlightedObject = null;

    private Outline outline;

    private DroneController droneController;
    private bool isInteracting = false;

    private bool interactionSucceed;

    private Vector3 initialPosition;
    private GameObject drone;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 싱글톤 인스턴스를 씬 전환 시에도 유지
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트에 핸들러 등록
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 1f, Color.red);

        if (Physics.Raycast(ray, out hit, 1f, LayerMask.GetMask("interactable")) && !isInteracting)
        {
            outline = hit.collider.gameObject.GetComponent<Outline>();
            outline.enabled = true;
            if (Input.GetKeyDown(KeyCode.F))
            {
                isInteracting = true;

                initialPosition = drone.transform.position;
                mainCamera.transform.position = new Vector3(-0.9679f, 0.06713f, 6.8644f);
                mainCamera.transform.rotation = Quaternion.Euler(new Vector3(21.725f, -0.304f, 0f));
                droneController.DisableControl();
            }
        }
        else
        {
            if (outline is not null)
            {
                outline.enabled = false;
            }
            // ClearHighlight();
        }

    }

    public void InteractionSucceed()
    {
        isInteracting = false;
        Debug.Log("Mission complete");
        droneController.EnableControl();
        drone.transform.position = initialPosition;
    }

    public void escapeInteraction()
    {
        isInteracting = false;
        Debug.Log("UI escaped");
        droneController.EnableControl();
        drone.transform.position = initialPosition;
    }
    // void SetHighlight(GameObject obj)
    // {
    //     Debug.Log(obj);
    //     // Code to enable glow on obj
    //     highlightedObject = obj;
    // }

    // void ClearHighlight()
    // {
    //     if (highlightedObject != null)
    //     {
    //         // Code to disable glow on highlightedObject
    //         highlightedObject = null;
    //     }
    // }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 이벤트 해제
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject[] interactableObjects = GameObject.FindGameObjectsWithTag("Interactable");
        for (int i = 0; i < interactableObjects.Length; i++)
        {
            interactableObjects[i].GetComponent<Outline>().enabled = false;
        }
        // 씬이 로드될 때마다 메인 카메라를 다시 찾기
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // 새 카메라에 상호작용 스크립트 추가 또는 카메라 참조 업데이트
            Debug.Log("New Camera Loaded in Scene: " + scene.name);
        }
        droneController = GameObject.Find("Drone").GetComponent<DroneController>();
        drone = GameObject.Find("Drone");
    }

    public void passwordSuccess()
    {

    }

    public Camera GetMainCamera()
    {
        return mainCamera;
    }
}