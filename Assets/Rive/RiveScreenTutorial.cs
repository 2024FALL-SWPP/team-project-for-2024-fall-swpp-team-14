using System;
using System.Collections.Concurrent;
using System.Linq;
using Rive;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
public class RiveScreenTutorial : MonoBehaviour
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

    //For Fetching Inputs
    SMITrigger nextNarration;
    SMIBool mission1Complete;
    SMIBool mission2Complete1;
    SMIBool mission2Complete2;
    SMIBool mission3Complete;
    public int missionInt = 0;


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
        m_helper?.UpdateTextureHelper();
        if (m_artboard == null)
        {
            return;
        }

        //fetching inputs
        nextNarration = m_stateMachine.GetTrigger("nextNarration");
        mission1Complete = m_stateMachine.GetBool("mission1_complete");
        mission2Complete1 = m_stateMachine.GetBool("mission2_complete1");
        mission2Complete2 = m_stateMachine.GetBool("mission2_complete2");
        mission3Complete = m_stateMachine.GetBool("mission3_complete");

        Camera camera = gameObject.GetComponent<Camera>();
        if (camera != null)
        {
            Vector3 mousePos = camera.ScreenToViewportPoint(Input.mousePosition);
            Vector2 mouseRiveScreenPos = new Vector2(
                mousePos.x * camera.pixelWidth,
                (1 - mousePos.y) * camera.pixelHeight
            );
            if (m_lastMousePosition != mouseRiveScreenPos)
            {
                Vector2 local = m_artboard.LocalCoordinate(
                    mouseRiveScreenPos,
                    new Rect(0, 0, camera.pixelWidth, camera.pixelHeight),
                    fit,
                    alignment
                );
                m_stateMachine?.PointerMove(local);
                m_lastMousePosition = mouseRiveScreenPos;
            }
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 local = m_artboard.LocalCoordinate(
                    mouseRiveScreenPos,
                    new Rect(0, 0, camera.pixelWidth, camera.pixelHeight),
                    fit,
                    alignment
                );
                m_stateMachine?.PointerDown(local);
                m_wasMouseDown = true;
            }
            else if (m_wasMouseDown)
            {
                m_wasMouseDown = false;
                Vector2 local = m_artboard.LocalCoordinate(
                    mouseRiveScreenPos,
                    new Rect(0, 0, camera.pixelWidth, camera.pixelHeight),
                    fit,
                    alignment
                );
                m_stateMachine?.PointerUp(local);
            }
        }

        // Find reported Rive events before calling advance.
        foreach (var report in m_stateMachine?.ReportedEvents() ?? Enumerable.Empty<ReportedEvent>())
        {
            OnRiveEvent?.Invoke(report);
        }

        m_stateMachine?.Advance(Time.deltaTime);

        Debug.Log("next narration: " + nextNarration);

        // if (nextNarration) //whenever nextNarration is triggered, increment missionInt
        // {
        //     missionInt++;
        // }

    }

    private void OnDisable()
    {
        Camera camera = gameObject.GetComponent<Camera>();
        if (m_commandBuffer != null && camera != null)
        {
            camera.RemoveCommandBuffer(cameraEvent, m_commandBuffer);
        }

    }
}