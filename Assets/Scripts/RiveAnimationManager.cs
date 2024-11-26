using System;
using System.Collections.Concurrent;
using System.Linq;
using Rive;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

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
public class RiveAnimationManager : MonoBehaviour
{
    public Rive.Asset[] asset_list;
    public CameraEvent cameraEvent = CameraEvent.AfterEverything;
    public Fit fit = Fit.Contain;
    public Alignment alignment = Alignment.Center;
    public event RiveEventDelegate OnRiveEvent;
    public delegate void RiveEventDelegate(ReportedEvent reportedEvent);

    private Rive.RenderQueue[] m_renderQueue = new Rive.RenderQueue[4];
    private Rive.Renderer[] m_riveRenderer = new Rive.Renderer[4];
    private CommandBuffer[] m_commandBuffer = new CommandBuffer[4];

    private Rive.File[] m_file = new Rive.File[4];
    private Artboard[] m_artboard = new Artboard[4];
    private StateMachine[] m_stateMachine = new StateMachine[4];
    private CameraTextureHelper[] m_helper = new CameraTextureHelper[4];

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
        for (int i = 0; i < 4; i++){
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
        // if (m_helper != null && Event.current.type.Equals(EventType.Repaint))
        // {
        //     var texture = m_helper.renderTexture;

        //     var width = m_helper.camera.scaledPixelWidth;
        //     var height = m_helper.camera.scaledPixelHeight;

        //     GUI.DrawTexture(
        //         flipY() ? new Rect(0, height, width, -height) : new Rect(0, 0, width, height),
        //         texture,
        //         ScaleMode.StretchToFill,
        //         true
        //     );

        // }

    }

    private void Awake()
    {
        Camera camera = gameObject.GetComponent<Camera>();
        Assert.IsNotNull(camera, "RiveScreen must be attached to a camera.");
        for (int i = 0; i < 4; i++){
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
        // if (asset_list[0] != null)
        // {
        //     m_file = Rive.File.Load(asset_list[0]);
        //     m_artboard = m_file.Artboard(0);
        //     m_stateMachine = m_artboard?.StateMachine();
        // }

        // Make a RenderQueue that doesn't have a backing texture and does not
        // clear the target (we'll be drawing on top of it).
        // m_renderQueue = new Rive.RenderQueue(null, false);
        // m_riveRenderer = m_renderQueue.Renderer();
        // m_commandBuffer = m_riveRenderer.ToCommandBuffer();

        // if (!Rive.RenderQueue.supportsDrawingToScreen())
        // {
        //     m_helper = new CameraTextureHelper(camera, m_renderQueue);
        //     m_commandBuffer.SetRenderTarget(m_helper.renderTexture);
        // }
        // camera.AddCommandBuffer(cameraEvent, m_commandBuffer);

        // DrawRive(m_renderQueue);
    }

    void DrawRive(int i)
    {
        // if (m_artboard == null)
        // {
        //     return;
        // }
        // m_riveRenderer.Align(fit, alignment ?? Alignment.Center, m_artboard);
        // m_riveRenderer.Draw(m_artboard);

        if (m_artboard[i] == null)
        {
            return;
        }
        m_riveRenderer[i].Align(fit, alignment ?? Alignment.Center, m_artboard[i]);
        m_riveRenderer[i].Draw(m_artboard[i]);
    }

    private Vector2 m_lastMousePosition;
    bool m_wasMouseDown = false;

    private void Update()
    {
        SMITrigger someTrigger = m_stateMachine[3].GetTrigger("Is_Active");
        if (someTrigger != null)
        {
            someTrigger.Fire();
        }
        someTrigger = m_stateMachine[3].GetTrigger("Is_Checked");
        if (someTrigger != null)
        {
            someTrigger.Fire();
        }
        // Debug.Log(Time.time);
        // if (Time.time > 1.0f && Time.time < 4.0f && !isfired)
        // {
        //     m_file = Rive.File.Load(asset_list[0]);
        //     m_artboard = m_file.Artboard(0);
        //     m_stateMachine = m_artboard?.StateMachine();
        //     SMITrigger someTrigger = m_stateMachine.GetTrigger("Is_Active");
        //     someTrigger.Fire();
        //     m_stateMachine?.Advance(Time.deltaTime);
        //     isfired = true;
        // }
        // else if (Time.time >= 4.0f && Time.time < 6.0f)
        // {
        //     m_file = Rive.File.Load(asset_list[0]);
        //     m_artboard = m_file.Artboard(0);
        //     m_stateMachine = m_artboard?.StateMachine();
        //     SMITrigger someTrigger = m_stateMachine.GetTrigger("Is_Checked");
        //     m_stateMachine?.Advance(Time.deltaTime);
        //     someTrigger.Fire();
        // }
        // else if (Time.time >= 6.0f && Time.time < 10.0f)
        // {
        //     m_file = Rive.File.Load(asset_list[1]);
        //     m_artboard = m_file.Artboard(0);
        //     m_stateMachine = m_artboard?.StateMachine();
        //     SMITrigger someTrigger = m_stateMachine.GetTrigger("Is_Active");
        //     m_stateMachine?.Advance(Time.deltaTime);
        //     someTrigger.Fire();
        // }

        // m_helper?.UpdateTextureHelper();
        // if (m_artboard == null)
        // {
        //     return;
        // }

        Camera camera = gameObject.GetComponent<Camera>();
        if (camera != null)
        {
            for (int i = 0; i < 4; i++){
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
            // Vector3 mousePos = camera.ScreenToViewportPoint(Input.mousePosition);
            // Vector2 mouseRiveScreenPos = new Vector2(
            //     mousePos.x * camera.pixelWidth,
            //     (1 - mousePos.y) * camera.pixelHeight
            // );
            // if (m_lastMousePosition != mouseRiveScreenPos)
            // {
            //     Vector2 local = m_artboard.LocalCoordinate(
            //         mouseRiveScreenPos,
            //         new Rect(0, 0, camera.pixelWidth, camera.pixelHeight),
            //         fit,
            //         alignment
            //     );
            //     m_stateMachine?.PointerMove(local);
            //     m_lastMousePosition = mouseRiveScreenPos;
            // }
            // if (Input.GetMouseButtonDown(0))
            // {
            //     Vector2 local = m_artboard.LocalCoordinate(
            //         mouseRiveScreenPos,
            //         new Rect(0, 0, camera.pixelWidth, camera.pixelHeight),
            //         fit,
            //         alignment
            //     );
            //     m_stateMachine?.PointerDown(local);
            //     m_wasMouseDown = true;
            // }
            // else if (m_wasMouseDown)
            // {
            //     m_wasMouseDown = false;
            //     Vector2 local = m_artboard.LocalCoordinate(
            //         mouseRiveScreenPos,
            //         new Rect(0, 0, camera.pixelWidth, camera.pixelHeight),
            //         fit,
            //         alignment
            //     );
            //     m_stateMachine?.PointerUp(local);
            // }

            // m_stateMachine?.Advance(Time.deltaTime);
        }

        // Find reported Rive events before calling advance.
        for (int i = 0; i < 4; i++){
            foreach (var report in m_stateMachine[i]?.ReportedEvents() ?? Enumerable.Empty<ReportedEvent>())
            {
                OnRiveEvent?.Invoke(report);
            }
        }
        // foreach (var report in m_stateMachine?.ReportedEvents() ?? Enumerable.Empty<ReportedEvent>())
        // {
        //     OnRiveEvent?.Invoke(report);
        // }
    }

    private void OnDisable()
    {
        Camera camera = gameObject.GetComponent<Camera>();
        for (int i = 0; i < 4; i++){
            if (m_commandBuffer[i] != null && camera != null)
            {
                camera.RemoveCommandBuffer(cameraEvent, m_commandBuffer[i]);
            }
        }
        // if (m_commandBuffer != null && camera != null)
        // {
        //     camera.RemoveCommandBuffer(cameraEvent, m_commandBuffer);
        // }

    }
}