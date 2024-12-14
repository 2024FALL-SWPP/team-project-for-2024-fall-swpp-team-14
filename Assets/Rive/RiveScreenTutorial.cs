using System;
using System.Collections.Concurrent;
using System.Linq;
using Rive;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using System.Collections;

public class RiveScreenTutorial : MonoBehaviour
{
    public Rive.Asset[] asset_list;
    public CameraEvent cameraEvent = CameraEvent.AfterEverything;
    public Fit fit = Fit.Contain;
    public Alignment alignment = Alignment.Center;
    public event RiveEventDelegate OnRiveEvent;
    public delegate void RiveEventDelegate(ReportedEvent reportedEvent);

    private Rive.RenderQueue[] m_renderQueue = new Rive.RenderQueue[3];
    private Rive.Renderer[] m_riveRenderer = new Rive.Renderer[3];
    private CommandBuffer[] m_commandBuffer = new CommandBuffer[3];

    private Rive.File[] m_file = new Rive.File[3];
    private Artboard[] m_artboard = new Artboard[3];
    private StateMachine[] m_stateMachine = new StateMachine[3];
    private CameraTextureHelper[] m_helper = new CameraTextureHelper[3];
    public DroneController droneController;

    //public StateMachine stateMachine => m_stateMachine; //state machine is changed to being public

    public SMINumber alertCount;
    public SMINumber hp;
    public SMINumber ammo;
    public SMIBool[] missionBools = new SMIBool[4];

    public SMIBool[] skipStarts = new SMIBool[2];
    private SMITrigger spacebarTrigger;

    private float animationTime = 0.0f;


    //public bool[] isTutorialMissionCleared = new bool[4] { false, false, false, false };

    //For Fetching Inputs
    public int narrationInt = 0; //switch to private later?
    private GameObject drone;

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
        for (int i = 0; i < 3; i++)
        {
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
        Camera camera = gameObject.GetComponent<Camera>();
        Assert.IsNotNull(camera, "RiveScreen must be attached to a camera.");

        for (int i = 0; i < 3; i++)
        {
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
        OnRiveEvent += RiveScreen_OnRiveEvent;

        drone = GameObject.FindWithTag("Player");
        if (DataTransfer.skiptoTutorial3)
        {
            drone.transform.position = new Vector3(3, 0, -7);
            drone.transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (DataTransfer.skiptoTutorial2)
        {
            drone.transform.position = new Vector3(6, -0.5f, -3);
            drone.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (DataTransfer.skiptoTutorial1)
        {
            drone.transform.position = new Vector3(11, 0, 6);
            drone.transform.rotation = Quaternion.Euler(0, -90, 0);
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

    }

    void DrawRive(int i)
    {
        if (m_artboard[i] == null)
        {
            return;
        }
        if (i == 2 || i == 4)
        {
            m_riveRenderer[i].Align(Fit.None, Alignment.TopRight, m_artboard[i]);
        }
        else if (i == 1)
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

    void Start()
    {
        skipStarts[0] = m_stateMachine[0].GetBool("startAtMission2");
        skipStarts[1] = m_stateMachine[0].GetBool("startAtMission3");
        spacebarTrigger = m_stateMachine[0].GetTrigger("nextNarration");
        if (DataTransfer.skiptoTutorial3)
        {
            setNarrationInt(15);
            skipStarts[1].Value = true;
        }
        else if (DataTransfer.skiptoTutorial2)
        {
            setNarrationInt(11);
            skipStarts[0].Value = true;
        }
        else if (DataTransfer.skiptoTutorial3)
        {
            setNarrationInt(15);
            skipStarts[1].Value = true;
        }
    }

    private void Update()
    {
        // m_helper?.UpdateTextureHelper();
        // if (m_artboard == null)
        // {
        //     return;
        // }

        //fetching inputs

        if (Input.GetKeyDown(KeyCode.Space) && spacebarTrigger != null)
        {
            spacebarTrigger.Fire();
        }

        missionBools[0] = m_stateMachine[0].GetBool("mission1_complete");
        missionBools[1] = m_stateMachine[0].GetBool("mission2_complete1");
        missionBools[2] = m_stateMachine[0].GetBool("mission2_complete2");
        missionBools[3] = m_stateMachine[0].GetBool("mission3_complete");

        ammo = m_stateMachine[1].GetNumber("ammo");
        hp = m_stateMachine[2].GetNumber("hp");
        hp.Value = (int)(droneController.droneHp / 10);
        ammo.Value = droneController.currentReloadCnt;

        if (droneController.droneHp == 0)
        {
            this.enabled = false;
        }

        Camera camera = gameObject.GetComponent<Camera>();
        if (camera != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if (m_artboard[i] == null || m_stateMachine[i] == null /* || m_helper[i] == null*/)
                {
                    continue; // Skip uninitialized elements
                }
                m_helper[i]?.UpdateTextureHelper();
                if (m_artboard[i] == null) //used to be: if (m_artboard == null)
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
                //m_stateMachine[i]?.Advance(Time.deltaTime);
            }

            if (getNarrationInt() >= 14)
            {
                DataTransfer.skiptoTutorial3 = true;
            }
            else if (getNarrationInt() >= 11)
            {
                DataTransfer.skiptoTutorial2 = true;
            }

        }

        // Find reported Rive events before calling advance.
        for (int i = 0; i < 3; i++)
        {
            foreach (var report in m_stateMachine[i]?.ReportedEvents() ?? Enumerable.Empty<ReportedEvent>())
            {
                OnRiveEvent?.Invoke(report);
                //RiveScreen_OnRiveEvent(report);
            }
            m_stateMachine[i]?.Advance(Time.deltaTime);
        }

        for (int i = 0; i < 3; i++)
        {
            if (m_stateMachine[i] != null)
            {
                m_stateMachine[i].Advance(Time.deltaTime);
            }
        }

        if (missionBools[3].Value)
        {
            StartCoroutine(WaitAndMapClear());
        }

    }

    private void OnDisable()
    {
        Camera camera = gameObject.GetComponent<Camera>();
        for (int i = 0; i < 3; i++)
        {
            if (m_commandBuffer[i] != null && camera != null)
            {
                camera.RemoveCommandBuffer(cameraEvent, m_commandBuffer[i]);
            }
        }
        if (m_stateMachine[0] != null) // Or use the relevant index or artboard
        {
            animationTime += Time.deltaTime;
        }

    }

    private void RiveScreen_OnRiveEvent(ReportedEvent reportedEvent)
    {
        //Debug.Log($"Event received, name: \"{reportedEvent.Name}\", secondsDelay: {reportedEvent.SecondsDelay}");

        // Access specific event properties
        if (reportedEvent.Name == "nextNarrationEvent")
        {
            narrationInt++;
        }
    }

    public int getNarrationInt()
    {
        return this.narrationInt;
    }

    public void setNarrationInt(int i)
    {
        this.narrationInt = i;
    }

    void OnEnable()
    {
        if (m_stateMachine[0] != null) // Or use the relevant index or artboard
        {
            //m_stateMachine[0].Advance(animationTime);
        }
        Camera camera = gameObject.GetComponent<Camera>();
        // for (int i = 0; i < 3; i++)
        // {
        //     if (m_commandBuffer[i] != null && camera != null)
        //     {
        //         camera.AddCommandBuffer(cameraEvent, m_commandBuffer[i]);
        //     }
        // }
    }

    IEnumerator WaitAndMapClear()
    {
        yield return new WaitForSeconds(3f);
        droneController.MapClear();
        this.enabled = false;
    }
}