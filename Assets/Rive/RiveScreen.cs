using System;
using System.Collections.Concurrent;
using System.Linq;
using Rive;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

internal class CameraTextureHelper
{
    private Camera m_camera;
    private RenderTexture m_renderTexture;
    private int m_pixelWidth = -1;
    private int m_pixelHeight = -1;
    private Rive.RenderQueue m_renderQueue;

    // Queue to keep things on the main thread only.
    private static ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();

    public RenderTexture renderTexture
    {
        get { return m_renderTexture; }
    }

    public Camera camera
    {
        get { return m_camera; }
    }

    internal CameraTextureHelper(Camera camera, Rive.RenderQueue queue)
    {
        m_camera = camera;
        m_renderQueue = queue;
        UpdateTextureHelper();
    }

    ~CameraTextureHelper()
    {
        // Since the GC calls the destructor and doesn't run on the main thread,
        // we need to ensure the cleanup() call happens on the main thread.
        mainThreadActions.Enqueue(() => Cleanup());
    }

    void Cleanup()
    {
        if (m_renderTexture != null)
        {
            m_renderTexture.Release();
        }
    }

    private void Update()
    {
        // Process main thread actions
        while (mainThreadActions.TryDequeue(out var action))
        {
            action();
        }
    }

    public bool UpdateTextureHelper()
    {

        if (m_pixelWidth == m_camera.pixelWidth && m_pixelHeight == m_camera.pixelHeight)
        {
            return false;
        }

        Cleanup();

        m_pixelWidth = m_camera.pixelWidth;
        m_pixelHeight = m_camera.pixelHeight;
        var textureDescriptor = TextureHelper.Descriptor(m_pixelWidth, m_pixelHeight);
        m_renderTexture = new RenderTexture(textureDescriptor);
        m_renderTexture.Create();
        m_renderQueue.UpdateTexture(m_renderTexture);

        return true;
    }

}

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
// Draw a Rive artboard to the screen. Must be bound to a camera.
public class RiveScreen : MonoBehaviour
{
    public Rive.Asset asset;
    public CameraEvent cameraEvent = CameraEvent.AfterEverything;
    public Fit fit = Fit.Contain;
    public Alignment alignment = Alignment.Center;
    public event RiveEventDelegate OnRiveEvent;
    public delegate void RiveEventDelegate(ReportedEvent reportedEvent);

    private Rive.RenderQueue m_renderQueue;
    private Rive.Renderer m_riveRenderer;
    private CommandBuffer m_commandBuffer;

    private Rive.File m_file;
    private Artboard m_artboard;
    private StateMachine m_stateMachine;
    private CameraTextureHelper m_helper;

    public StateMachine stateMachine => m_stateMachine;

    private SMINumber moveToMission; //input from rive
    private Vector3 landPosition;
    private Quaternion landRotation;
    public RiveScreenTutorial riveScreenTutorial;


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
        if (m_helper != null && Event.current.type.Equals(EventType.Repaint))
        {
            var texture = m_helper.renderTexture;

            var width = m_helper.camera.scaledPixelWidth;
            var height = m_helper.camera.scaledPixelHeight;

            GUI.DrawTexture(
                flipY() ? new Rect(0, height, width, -height) : new Rect(0, 0, width, height),
                texture,
                ScaleMode.StretchToFill,
                true
            );

        }

    }

    private void Awake()
    {
        if (asset != null)
        {
            m_file = Rive.File.Load(asset);
            m_artboard = m_file.Artboard(0);
            m_stateMachine = m_artboard?.StateMachine();
        }

        Camera camera = gameObject.GetComponent<Camera>();
        Assert.IsNotNull(camera, "RiveScreen must be attached to a camera.");

        // Make a RenderQueue that doesn't have a backing texture and does not
        // clear the target (we'll be drawing on top of it).
        m_renderQueue = new Rive.RenderQueue(null, false);
        m_riveRenderer = m_renderQueue.Renderer();
        m_commandBuffer = m_riveRenderer.ToCommandBuffer();

        if (!Rive.RenderQueue.supportsDrawingToScreen())
        {
            m_helper = new CameraTextureHelper(camera, m_renderQueue);
            m_commandBuffer.SetRenderTarget(m_helper.renderTexture);
        }
        camera.AddCommandBuffer(cameraEvent, m_commandBuffer);

        DrawRive(m_renderQueue);
    }

    void DrawRive(Rive.RenderQueue queue)
    {
        if (m_artboard == null)
        {
            return;
        }
        m_riveRenderer.Align(fit, alignment ?? Alignment.Center, m_artboard);
        m_riveRenderer.Draw(m_artboard);

    }

    private Vector2 m_lastMousePosition;
    bool m_wasMouseDown = false;

    private void Update()
    {
        m_helper?.UpdateTextureHelper(); //Ensures that the render texture associated with the camera is up-to-date
        if (m_artboard == null) //If the m_artboard is null, the function exits early.
        {
            return;
        }

        moveToMission = m_stateMachine.GetNumber("moveToMission"); //fetch input from rive
        if (moveToMission == null) return;
        if (moveToMission.Value != 0)
        {
            Debug.Log("move to mission: " + moveToMission.Value);
            switch (moveToMission.Value)
            {
                case 1: //Tutorial Mission 1
                    landPosition = new Vector3(11, 0, 6);
                    landRotation = Quaternion.Euler(0, -90, 0);
                    SceneManager.LoadSceneAsync("TutorialScene", LoadSceneMode.Single).completed += OnSceneLoaded;
                    break;
                case 2: //Tutorial Mission2
                    landPosition = new Vector3(6, -0.5f, -3);
                    landRotation = Quaternion.Euler(0, 180, 0);
                    DataTransfer.skiptoTutorial2 = true;
                    SceneManager.LoadSceneAsync("TutorialScene", LoadSceneMode.Single).completed += OnSceneLoaded;
                    break;
                case 3: //Tutorial Mission 3
                    landPosition = new Vector3(3, 0, -7);
                    landRotation = Quaternion.Euler(0, -90, 0);
                    DataTransfer.skiptoTutorial3 = true;
                    SceneManager.LoadSceneAsync("TutorialScene", LoadSceneMode.Single).completed += OnSceneLoaded;
                    break;
                case 4: //MainMap Mission 1
                    landPosition = new Vector3(68, 2, 20);
                    landRotation = Quaternion.Euler(0, 180, 0);
                    SceneManager.LoadSceneAsync("Map_v2", LoadSceneMode.Single).completed += OnSceneLoaded;

                    break;
                case 5: //MainMap Mission 2
                    landPosition = new Vector3(55, 5, -13);
                    landRotation = Quaternion.Euler(0, 0, 0);
                    SceneManager.LoadSceneAsync("Map_v2", LoadSceneMode.Single).completed += OnSceneLoaded;
                    break;
                case 6: //MainMap Mission 3
                    landPosition = new Vector3(42, 11, -13);
                    landRotation = Quaternion.Euler(0, 130, 0);
                    SceneManager.LoadSceneAsync("Map_v2", LoadSceneMode.Single).completed += OnSceneLoaded;
                    break;
                default:
                    break;
            }
            moveToMission.Value = 0;
        }

        Camera camera = gameObject.GetComponent<Camera>();
        if (camera != null)
        {
            Vector3 mousePos = camera.ScreenToViewportPoint(Input.mousePosition); //Converts current mouse position: screen space --> viewport space
            Vector2 mouseRiveScreenPos = new Vector2( //Map mouse position to the artboard's coordinate system using the viewport and camera dimensions.
                mousePos.x * camera.pixelWidth,
                (1 - mousePos.y) * camera.pixelHeight
            );
            if (m_lastMousePosition != mouseRiveScreenPos) //if the mouse position has changed,
            {
                Vector2 local = m_artboard.LocalCoordinate( //Convert the mouse position to the local coordinate system of Rive artboard using LocalCoordinate
                    mouseRiveScreenPos,
                    new Rect(0, 0, camera.pixelWidth, camera.pixelHeight),
                    fit,
                    alignment
                );
                m_stateMachine?.PointerMove(local); //Call PointerMove() on the m_stateMachine to notify Rive animation of the mouse movement.
                m_lastMousePosition = mouseRiveScreenPos; //Update m_lastMousePosition to the new position
            }
            if (Input.GetMouseButtonDown(0)) // if Left mouse button pressed
            {
                Vector2 local = m_artboard.LocalCoordinate( //Converts mouse position to Rive's coordinated & call PointerDown() on the state machine.
                    mouseRiveScreenPos, //mouse position mapped to the artboard's coordinate system
                    new Rect(0, 0, camera.pixelWidth, camera.pixelHeight), //bounding area of the Rive artboard in screen space
                    fit, //scaling/fitting mode used to align Rive artboard within the camera's viewport.
                    alignment //specifies how the artboard is aligned within the viewport when its aspect ratio doesn't match the screen's aspect ratio.
                );
                m_stateMachine?.PointerDown(local);
                m_wasMouseDown = true; //Set m_wasMouseDown to true to track the state.
            }
            else if (m_wasMouseDown) //Detects when the mouse button is released (from a previously pressed state).
            {
                m_wasMouseDown = false;
                Vector2 local = m_artboard.LocalCoordinate( //Converts the mouse position
                    mouseRiveScreenPos,
                    new Rect(0, 0, camera.pixelWidth, camera.pixelHeight),
                    fit,
                    alignment
                );
                m_stateMachine?.PointerUp(local); //calls PointerUp() on the state machine.
            }
        }

        // Find reported Rive events before calling advance.
        foreach (var report in m_stateMachine?.ReportedEvents() ?? Enumerable.Empty<ReportedEvent>())
        {
            //Retrieves events reported by the Rive StateMachine.
            OnRiveEvent?.Invoke(report);
        }

        m_stateMachine?.Advance(Time.deltaTime); //Updates the animation to reflect changes in time

    }

    private void OnDisable()
    {
        Camera camera = gameObject.GetComponent<Camera>();
        if (m_commandBuffer != null && camera != null)
        {
            camera.RemoveCommandBuffer(cameraEvent, m_commandBuffer);
        }

    }

    private void OnSceneLoaded(AsyncOperation asyncOperation)
    {
        GameObject drone = GameObject.FindWithTag("Player");
        if (drone != null)
        {
            drone.transform.position = landPosition;
            drone.transform.rotation = landRotation;
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
        else
        {
            Debug.LogError("Drone not found in the new scene!");
        }
    }
}