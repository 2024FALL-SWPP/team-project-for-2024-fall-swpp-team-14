using System;
using System.Collections.Concurrent;
using System.Linq;
using Rive;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
// Draw a Rive artboard to the screen. Must be bound to a camera.
public class RiveAnimationManager : MonoBehaviour
{
    public Rive.Asset[] asset_list;
    public CameraEvent cameraEvent = CameraEvent.AfterEverything;
    public Fit fit = Fit.Contain;
    public Alignment alignment = Alignment.Center;
    public event RiveEventDelegate OnRiveEvent;
    public delegate void RiveEventDelegate(ReportedEvent reportedEvent);

    private Rive.RenderQueue[] m_renderQueue = new Rive.RenderQueue[7];
    private Rive.Renderer[] m_riveRenderer = new Rive.Renderer[7];
    private CommandBuffer[] m_commandBuffer = new CommandBuffer[7];

    private Rive.File[] m_file = new Rive.File[7];
    private Artboard[] m_artboard = new Artboard[7];
    private StateMachine[] m_stateMachine = new StateMachine[7];
    private CameraTextureHelper[] m_helper = new CameraTextureHelper[7];

    public GameObject player;
    private DroneController droneController;
    private GameObject EnemyGenerator;
    private int maxAlert;

    SMITrigger[] isActive = new SMITrigger[4];
    SMITrigger[] isChecked = new SMITrigger[4];
    public SMINumber alertCount;
    public SMINumber hp;
    public SMINumber ammo;

    public bool[] isMainMapMissionCleared = new bool[4] { false, false, false, false };

    int currentMission = 0;
    float missionChangedTime = -1f;
    private GameObject drone;
    private MainMapManager mainMapManager;


    // public StateMachine stateMachine => m_stateMachine;

    private static bool flipY()
    {
        switch (UnityEngine.SystemInfo.graphicsDeviceType)
        {
            case UnityEngine.Rendering.GraphicsDeviceType.Metal:
            case UnityEngine.Rendering.GraphicsDeviceType.Direct3D11:
                return true;
            default:
                return false;
        }
    }

    void OnGUI()
    {
        for (int i = 0; i < 7; i++){
            if (m_helper[i] != null && Event.current.type.Equals(EventType.Repaint))
            {
                var texture = m_helper[i].renderTexture;

                var width = m_helper[i].camera.scaledPixelWidth;
                var height = m_helper[i].camera.scaledPixelHeight;

                GUI.DrawTexture(
                    flipY() ? new Rect(0, height, width, -height) : new Rect(0, 0, width, height),
                    texture,
                    ScaleMode.StretchToFill,
                    true
                );

            }
        }
    }

    private void Awake()
    {
        isMainMapMissionCleared[0] = DataTransfer.skiptoMainmap2 || DataTransfer.skiptoMainmap3;
        isMainMapMissionCleared[1] = DataTransfer.skiptoMainmap2 || DataTransfer.skiptoMainmap3;
        isMainMapMissionCleared[2] = DataTransfer.skiptoMainmap3;

        if (GameObject.Find("MainMapManager") != null)
        {
            mainMapManager = GameObject.Find("MainMapManager").GetComponent<MainMapManager>();
        }
        else
        {
            mainMapManager = null;
        }

        drone = GameObject.FindWithTag("Player");
        if (DataTransfer.skiptoMainmap3)
        {
            drone.transform.position = new Vector3(42, 11, -13);
            drone.transform.rotation = Quaternion.Euler(0, 130, 0);
        }
        else if (DataTransfer.skiptoMainmap2)
        {
            drone.transform.position = new Vector3(55, 5, -13);
            drone.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (DataTransfer.skiptoMainmap1)
        {
            drone.transform.position = new Vector3(68, 2, 20);
            drone.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        Transform aircraft = drone.transform.Find("Aircraft1");
        if (aircraft != null)
        {
            // Reset the aircraft's local rotation
            Quaternion desiredGlobalRotation = Quaternion.Euler(-90, -180, -90);

            // Calculate the required local rotation for the aircraft
            Quaternion parentGlobalRotation = drone.transform.rotation;
            Quaternion requiredLocalRotation = Quaternion.Inverse(parentGlobalRotation) * desiredGlobalRotation;

            // Set the aircraft's local rotation
            aircraft.localRotation = requiredLocalRotation;
        }


        while (currentMission < 4 && isMainMapMissionCleared[currentMission])
        {
            currentMission++;
        }
        Camera camera = gameObject.GetComponent<Camera>();
        Assert.IsNotNull(camera, "RiveScreen must be attached to a camera.");
        for (int i = 0; i < 7; i++){
            if (asset_list[i] != null)
            {
                m_file[i] = Rive.File.Load(asset_list[i]);
                m_artboard[i] = m_file[i].Artboard(0);
                m_stateMachine[i] = m_artboard[i]?.StateMachine();
            }
            // Make a RenderQueue that doesn't have a backing texture and does not
            // clear the target (we'll be drawing on top of it).
            m_renderQueue[i] = new Rive.RenderQueue(null, false);
            m_riveRenderer[i] = m_renderQueue[i].Renderer();
            m_commandBuffer[i] = m_riveRenderer[i].ToCommandBuffer();

            if (!Rive.RenderQueue.supportsDrawingToScreen())
            {
                m_helper[i] = new CameraTextureHelper(camera, m_renderQueue[i]);
                m_commandBuffer[i].SetRenderTarget(m_helper[i].renderTexture);
            }
            camera.AddCommandBuffer(cameraEvent, m_commandBuffer[i]);

            DrawRive(i);
        }
        m_stateMachine[currentMission].GetTrigger("Is_Active").Fire();
    }

    void DrawRive(int i)
    {
        if (m_artboard[i] == null)
        {
            return;
        }
        if (i == 4 || i == 5)
        {
            m_riveRenderer[i].Align(Fit.None, Alignment.TopRight, m_artboard[i]);
        }
        else if (i == 6)
        {
            m_riveRenderer[i].Align(Fit.None, Alignment.BottomRight, m_artboard[i]);
        }
        else
        {
            m_riveRenderer[i].Align(Fit.None, Alignment.TopLeft, m_artboard[i]);
        }
        m_riveRenderer[i].Draw(m_artboard[i]);
    }

    private Vector2 m_lastMousePosition;
    bool m_wasMouseDown = false;

    private void Update()
    {
        player = GameObject.FindWithTag("Player");
        EnemyGenerator = GameObject.Find("EnemyGenerator");
        for (int i = 0; i < 4; i++)
        {
            isActive[i] = m_stateMachine[i].GetTrigger("Is_Active");
            isChecked[i] = m_stateMachine[i].GetTrigger("Is_Checked");
        }
        alertCount = m_stateMachine[4].GetNumber("Alert_count");
        hp = m_stateMachine[5].GetNumber("hp");
        ammo = m_stateMachine[6].GetNumber("ammo");
        droneController = player.GetComponent<DroneController>();
        maxAlert = EnemyGenerator.GetComponent<EnemyGenerator>().maxAlert;

        if (currentMission < 4 && isMainMapMissionCleared[currentMission])
        {
            if (currentMission == 1)
            {
                DataTransfer.skiptoMainmap2 = true;
            }
            else if (currentMission == 2)
            {
                DataTransfer.skiptoMainmap3 = true;
            }
            m_stateMachine[currentMission].GetTrigger("Is_Checked").Fire();
            if (missionChangedTime == -1f)
            {
                missionChangedTime = Time.time;
            }
            if (currentMission < 3 && missionChangedTime + 2f < Time.time)
            {
                m_stateMachine[currentMission + 1].GetTrigger("Is_Active").Fire();
                missionChangedTime = -1f;
                currentMission++;
            }
        }

        alertCount.Value = maxAlert;
        hp.Value = (int)(droneController.droneHp / 10);
        ammo.Value = droneController.currentReloadCnt;
        Camera camera = gameObject.GetComponent<Camera>();
        if (camera != null)
        {
            for (int i = 0; i < 7; i++){
                m_helper[i]?.UpdateTextureHelper();
                if (m_artboard == null)
                {
                    return;
                }
                Vector3 mousePos = camera.ScreenToViewportPoint(Input.mousePosition);
                Vector2 mouseRiveScreenPos = new Vector2(
                    mousePos.x * camera.pixelWidth,
                    (1 - mousePos.y) * camera.pixelHeight
                );
                if (m_lastMousePosition != mouseRiveScreenPos)
                {
                    Vector2 local = m_artboard[i].LocalCoordinate(
                        mouseRiveScreenPos,
                        new Rect(0, 0, camera.pixelWidth, camera.pixelHeight),
                        fit,
                        alignment
                    );
                    m_stateMachine[i]?.PointerMove(local);
                    m_lastMousePosition = mouseRiveScreenPos;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 local = m_artboard[i].LocalCoordinate(
                        mouseRiveScreenPos,
                        new Rect(0, 0, camera.pixelWidth, camera.pixelHeight),
                        fit,
                        alignment
                    );
                    m_stateMachine[i]?.PointerDown(local);
                    m_wasMouseDown = true;
                }
                else if (m_wasMouseDown)
                {
                    m_wasMouseDown = false;
                    Vector2 local = m_artboard[i].LocalCoordinate(
                        mouseRiveScreenPos,
                        new Rect(0, 0, camera.pixelWidth, camera.pixelHeight),
                        fit,
                        alignment
                    );
                    m_stateMachine[i]?.PointerUp(local);
                }

                m_stateMachine[i]?.Advance(Time.deltaTime);
            }
        }

        // Find reported Rive events before calling advance.
        for (int i = 0; i < 7; i++){
            foreach (var report in m_stateMachine[i]?.ReportedEvents() ?? Enumerable.Empty<ReportedEvent>())
            {
                OnRiveEvent?.Invoke(report);
            }
        }
    }

    private void LateUpdate()
    {
        
    }

    private void OnDisable()
    {
        Camera camera = gameObject.GetComponent<Camera>();
        for (int i = 0; i < 7; i++){
            if (m_commandBuffer[i] != null && camera != null)
            {
                camera.RemoveCommandBuffer(cameraEvent, m_commandBuffer[i]);
            }
        }
    }
}